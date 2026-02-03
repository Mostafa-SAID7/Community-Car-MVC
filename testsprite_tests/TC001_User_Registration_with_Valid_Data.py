import asyncio
import time
from playwright.async_api import async_playwright, TimeoutError as PlaywrightTimeoutError

async def run_test():
    """
    Test TC001: User Registration with Valid Data
    Verify that a new user can successfully register with valid and complete information.
    """
    pw = None
    browser = None
    context = None

    try:
        # Start Playwright
        pw = await async_playwright().start()

        # Launch browser
        browser = await pw.chromium.launch(
            headless=True,
            args=[
                "--window-size=1280,720",
                "--disable-dev-shm-usage",
                "--ipc=host",
                "--single-process"
            ],
        )

        # Create browser context
        context = await browser.new_context()
        context.set_default_timeout(30000)

        # Open new page
        page = await context.new_page()

        print("✓ Step 1: Navigate to localhost:5002")
        await page.goto("http://localhost:5002", wait_until="domcontentloaded", timeout=15000)
        await page.wait_for_timeout(2000)

        print("✓ Step 2: Click 'Sign Up' link")
        signup_link = page.locator('[data-testid="signup-link"]')
        if await signup_link.count() == 0:
            # Maybe we are already on login page or need to find it differently
            # Try direct navigation
            print("  ℹ Navigate directly to register page")
            await page.goto("http://localhost:5002/Account/Register", wait_until="domcontentloaded")
        else:
            await signup_link.click()

        print("✓ Step 3: Wait for registration form")
        await page.wait_for_selector('[data-testid="register-email"]', state="visible", timeout=10000)

        # Generate unique user
        timestamp = int(time.time())
        test_email = f"testuser_{timestamp}@communitycar.com"
        test_password = "Password123!"
        test_name = f"Test User {timestamp}"

        print(f"  Using email: {test_email}")

        print("✓ Step 4: Fill Full Name")
        await page.fill('[data-testid="register-fullname"]', test_name)

        print("✓ Step 5: Fill Email")
        await page.fill('[data-testid="register-email"]', test_email)

        print("✓ Step 6: Fill Password")
        await page.fill('[data-testid="register-password"]', test_password)

        print("✓ Step 7: Fill Confirm Password")
        await page.fill('[data-testid="register-confirm"]', test_password)
        
        # Take screenshot before submit
        # await page.screenshot(path='before_register.png')

        print("✓ Step 8: Submit Registration")
        await page.click('[data-testid="register-submit"]')

        print("✓ Step 9: Wait for result")
        await page.wait_for_timeout(5000)
        
        current_url = page.url
        print(f"  Current URL: {current_url}")
        
        # Check for validation errors
        validation_summary = await page.locator(".validation-summary-errors").all_text_contents()
        if validation_summary:
            print(f"❌ Validation Errors: {validation_summary}")
        
        # Logic: If successful, should redirect to Login (or Home depending on implementation)
        # The controller code says: return RedirectToAction("Login"); with success message
        
        if "login" in current_url.lower():
            print("✓ Redirected to Login page (likely success)")
            
            # Additional check: Success message
            content = await page.content()
            if "Registration successful" in content or "check your email" in content:
                 print("✅ TEST PASSED: Registration success message found")
            else:
                 print("ℹ Redirected to login, checking for success message...")
                 # Verify we can login now?
                 
        elif "feed" in current_url.lower() or "index" in current_url.lower():
             print("✅ TEST PASSED: Redirected to Feed/Index (Auto-login?)")
        else:
             print("❌ TEST FAILED: Unexpected URL or stayed on register page")
             await page.screenshot(path='register_failed.png')
             raise Exception(f"Registration failed - URL: {current_url}")

    except PlaywrightTimeoutError as e:
        print(f"❌ TEST FAILED: Timeout error - {str(e)}")
        raise
    except Exception as e:
        print(f"❌ TEST FAILED: {str(e)}")
        raise
    finally:
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())
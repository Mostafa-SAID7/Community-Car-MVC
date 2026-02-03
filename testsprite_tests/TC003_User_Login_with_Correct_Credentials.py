import asyncio
import time
from playwright.async_api import async_playwright, TimeoutError as PlaywrightTimeoutError

async def run_test():
    """
    Test TC003: User Login with Correct Credentials
    Check that users can log in successfully. 
    NOTE: Creates a fresh user first to ensure test reliability.
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

        # --- SETUP: REGISTER NEW USER ---
        print("\n--- SETUP: Creating Test User ---")
        timestamp = int(time.time())
        test_email = f"login_test_{timestamp}@communitycar.com"
        test_password = "Password123!"
        test_name = f"Login Test User {timestamp}"
        
        print(f"  User: {test_email}")
        
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")
        print("✓ Step 0: Click Sign Up link")
        await page.click('[data-testid="signup-link"]')
        
        print("✓ Step 0.5: Wait for register form")
        await page.wait_for_selector('[data-testid="register-fullname"]', state="visible", timeout=10000)
        
        await page.fill('[data-testid="register-fullname"]', test_name)
        await page.fill('[data-testid="register-email"]', test_email)
        await page.fill('[data-testid="register-password"]', test_password)
        await page.fill('[data-testid="register-confirm"]', test_password)
        await page.click('[data-testid="register-submit"]')
        
        # Wait for redirect to login or success
        await page.wait_for_timeout(3000)
        
        # If auto-logged in or redirected to login, proceed
        # We assume redirection to Login as per TC001 findings
        
        print("\n--- TEST: Verifying Login ---")
        
        # Ensure we are on login page or navigate there
        if "login" not in page.url.lower():
             print("  Navigate to Login page")
             await page.goto("http://localhost:5002/Account/Login", wait_until="domcontentloaded")
        
        print("✓ Step 1: Wait for login form")
        email_input = page.locator('[data-testid="login-email"]')
        await email_input.wait_for(state="visible", timeout=10000)

        print("✓ Step 2: Fill credentials")
        await email_input.fill(test_email)
        
        password_input = page.locator('[data-testid="login-password"]')
        await password_input.fill(test_password)

        print("✓ Step 3: Click login button")
        login_button = page.locator('[data-testid="login-submit"]')
        await login_button.click()

        print("✓ Step 4: Wait for navigation after login")
        await page.wait_for_timeout(5000)

        # Check result
        current_url = page.url
        print(f"  Current URL after login: {current_url}")

        if "login" not in current_url.lower():
            print("✅ TEST PASSED: User successfully logged in")
            
            # Simple content check
            content = await page.content()
            if "logout" in content.lower() or "sign out" in content.lower():
                print("  ✓ Verified: Logout option found")
        else:
            print("❌ TEST FAILED: Still on login page")
            
            # Debug info
            try:
                validation_summary = await page.locator(".validation-summary-errors").text_content()
                if validation_summary:
                    print(f"  Validation: {validation_summary}")
            except: pass
            
            raise Exception("Login failed")

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
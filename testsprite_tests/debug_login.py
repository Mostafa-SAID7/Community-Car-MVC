import asyncio
from playwright import async_api

async def debug_login():
    pw = None
    browser = None
    context = None

    try:
        pw = await async_api.async_playwright().start()
        browser = await pw.chromium.launch(headless=False)
        context = await browser.new_context()
        page = await context.new_page()

        print("🔍 Debugging login process...")

        # Navigate to the application
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")
        
        # Take screenshot of initial page
        await page.screenshot(path="testsprite_tests/debug_initial.png")
        print("📸 Initial page screenshot saved")
        
        # Check what page we're on
        title = await page.title()
        url = page.url
        print(f"📄 Page title: {title}")
        print(f"🌐 Current URL: {url}")
        
        # Look for login form
        login_form = await page.locator('#login-form').count()
        email_input = await page.locator('#login-email').count()
        password_input = await page.locator('#login-password').count()
        login_button = await page.locator('#login-button').count()
        
        print(f"🔍 Login form elements found:")
        print(f"  Login form: {login_form}")
        print(f"  Email input: {email_input}")
        print(f"  Password input: {password_input}")
        print(f"  Login button: {login_button}")
        
        if login_form > 0:
            print("📝 Attempting login...")
            
            # Fill login form
            await page.fill('#login-email', 'seed@communitycar.com')
            await page.fill('#login-password', 'Memo@3560')
            
            # Take screenshot before clicking login
            await page.screenshot(path="testsprite_tests/debug_before_login.png")
            print("📸 Before login screenshot saved")
            
            # Click login button
            await page.click('#login-button')
            
            # Wait a moment
            await asyncio.sleep(5)
            
            # Take screenshot after login attempt
            await page.screenshot(path="testsprite_tests/debug_after_login.png")
            print("📸 After login screenshot saved")
            
            # Check current state
            new_title = await page.title()
            new_url = page.url
            print(f"📄 New page title: {new_title}")
            print(f"🌐 New URL: {new_url}")
            
            # Look for error messages
            error_messages = await page.locator('.text-red-500, .error, .alert-danger, [class*="error"]').all_text_contents()
            if error_messages:
                print("❌ Error messages found:")
                for error in error_messages:
                    if error.strip():
                        print(f"  - {error.strip()}")
            else:
                print("ℹ️ No error messages found")
            
            # Check if still on login page
            still_login_form = await page.locator('#login-form').count()
            if still_login_form > 0:
                print("❌ Still on login page")
                
                # Check for validation errors
                validation_errors = await page.locator('.validation-summary-errors, [asp-validation-summary]').all_text_contents()
                if validation_errors:
                    print("⚠️ Validation errors:")
                    for error in validation_errors:
                        if error.strip():
                            print(f"  - {error.strip()}")
            else:
                print("✅ Successfully left login page")
        else:
            print("ℹ️ No login form found on initial page")
        
        # Check if we can access the home page directly
        print("\n🏠 Testing direct access to home page...")
        await page.goto("http://localhost:5002/", wait_until="domcontentloaded")
        await asyncio.sleep(2)
        
        home_title = await page.title()
        home_url = page.url
        print(f"📄 Home page title: {home_title}")
        print(f"🌐 Home URL: {home_url}")
        
        await page.screenshot(path="testsprite_tests/debug_home_direct.png")
        print("📸 Direct home access screenshot saved")

    except Exception as e:
        print(f"❌ Debug failed: {e}")
        import traceback
        traceback.print_exc()

    finally:
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(debug_login())
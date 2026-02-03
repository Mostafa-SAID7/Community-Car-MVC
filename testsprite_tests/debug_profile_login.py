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
        await page.screenshot(path="testsprite_tests/debug_profile_initial.png")
        print("📸 Initial page screenshot saved")
        
        # Check what page we're on
        title = await page.title()
        url = page.url
        print(f"📄 Page title: {title}")
        print(f"🌐 Current URL: {url}")
        
        # Look for login form elements
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
            await page.fill('#login-password', 'Password123!')
            
            # Take screenshot before clicking login
            await page.screenshot(path="testsprite_tests/debug_profile_before_login.png")
            print("📸 Before login screenshot saved")
            
            # Click login button
            await page.click('#login-button')
            
            # Wait a moment
            await asyncio.sleep(5)
            
            # Take screenshot after login attempt
            await page.screenshot(path="testsprite_tests/debug_profile_after_login.png")
            print("📸 After login screenshot saved")
            
            # Check current state
            new_title = await page.title()
            new_url = page.url
            print(f"📄 New page title: {new_title}")
            print(f"🌐 New URL: {new_url}")
            
            # Look for error messages
            error_selectors = [
                '.text-red-500',
                '.error',
                '.alert-danger',
                '[class*="error"]',
                '.validation-summary-errors'
            ]
            
            for selector in error_selectors:
                error_count = await page.locator(selector).count()
                if error_count > 0:
                    try:
                        error_text = await page.locator(selector).text_content()
                        if error_text and error_text.strip():
                            print(f"❌ Error found ({selector}): {error_text.strip()}")
                    except:
                        pass
            
            # Check if still on login page
            still_login_form = await page.locator('#login-form').count()
            if still_login_form > 0:
                print("❌ Still on login page")
                
                # Get page content for debugging
                page_text = await page.text_content('body')
                print(f"📄 Page content (first 500 chars): {page_text[:500] if page_text else 'No content'}")
                
            else:
                print("✅ Successfully left login page")
                
                # Look for profile/settings navigation
                nav_selectors = [
                    'a[href*="profile"]',
                    'a[href*="settings"]',
                    '[data-testid*="profile"]',
                    '[data-testid*="settings"]',
                    'button:has-text("Profile")',
                    'a:has-text("Settings")'
                ]
                
                print("🔍 Looking for navigation elements...")
                for selector in nav_selectors:
                    count = await page.locator(selector).count()
                    if count > 0:
                        try:
                            text = await page.locator(selector).first.text_content()
                            href = await page.locator(selector).first.get_attribute('href')
                            print(f"✅ Found nav element: {selector} - Text: '{text}' - Href: '{href}'")
                        except:
                            print(f"✅ Found nav element: {selector} (could not get details)")
        else:
            print("ℹ️ No login form found on initial page")

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
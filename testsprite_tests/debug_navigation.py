import asyncio
from playwright import async_api

async def debug_navigation():
    pw = None
    browser = None
    context = None

    try:
        pw = await async_api.async_playwright().start()
        browser = await pw.chromium.launch(headless=False)
        context = await browser.new_context()
        page = await context.new_page()

        print("🔍 Testing navigation to different pages...")

        # Test home page
        print("📍 Testing home page...")
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")
        title = await page.title()
        print(f"Home page title: {title}")
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/debug_home.png")
        
        # Test register page
        print("📍 Testing register page...")
        await page.goto("http://localhost:5002/register", wait_until="domcontentloaded")
        title = await page.title()
        print(f"Register page title: {title}")
        
        # Check if register form exists
        register_form = await page.locator('#register-form').count()
        fullname_input = await page.locator('#register-fullname').count()
        print(f"Register form found: {register_form > 0}")
        print(f"Fullname input found: {fullname_input > 0}")
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/debug_register.png")
        
        # Test login page
        print("📍 Testing login page...")
        await page.goto("http://localhost:5002/login", wait_until="domcontentloaded")
        title = await page.title()
        print(f"Login page title: {title}")
        
        # Check if login form exists
        login_form = await page.locator('#login-form').count()
        email_input = await page.locator('#login-email').count()
        print(f"Login form found: {login_form > 0}")
        print(f"Email input found: {email_input > 0}")
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/debug_login.png")
        
        # Test chat page (might require auth)
        print("📍 Testing chat page...")
        await page.goto("http://localhost:5002/chats", wait_until="domcontentloaded")
        title = await page.title()
        print(f"Chat page title: {title}")
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/debug_chat.png")
        
        print("✅ Navigation debug completed. Check screenshots for details.")

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
    asyncio.run(debug_navigation())
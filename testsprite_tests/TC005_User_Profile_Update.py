import asyncio
from playwright import async_api
from playwright.async_api import expect

async def run_test():
    pw = None
    browser = None
    context = None

    try:
        # Start a Playwright session in asynchronous mode
        pw = await async_api.async_playwright().start()

        # Launch a Chromium browser in headless mode with custom arguments
        browser = await pw.chromium.launch(
            headless=True,
            args=[
                "--window-size=1280,720",         # Set the browser window size
                "--disable-dev-shm-usage",        # Avoid using /dev/shm which can cause issues in containers
                "--ipc=host",                     # Use host-level IPC for better stability
                "--single-process"                # Run the browser in a single process mode
            ],
        )

        # Create a new browser context (like an incognito window)
        context = await browser.new_context()
        context.set_default_timeout(5000)

        # Open a new page in the browser context
        page = await context.new_page()

        # Navigate to your target URL and wait until the network request is committed
        await page.goto("http://localhost:5002", wait_until="commit", timeout=10000)

        # Wait for the main page to reach DOMContentLoaded state (optional for stability)
        try:
            await page.wait_for_load_state("domcontentloaded", timeout=3000)
        except async_api.Error:
            pass

        # Iterate through all iframes and wait for them to load as well
        for frame in page.frames:
            try:
                await frame.wait_for_load_state("domcontentloaded", timeout=3000)
            except async_api.Error:
                pass

        # Interact with the page elements to simulate user flow
        # -> Navigate to http://localhost:5002
        await page.goto("http://localhost:5002", wait_until="commit", timeout=10000)
        
        # -> Fill the Email/Username and Password fields and click the LogIn button to authenticate.
        frame = context.pages[-1]
        try:
            # Login with data-testid
            await page.get_by_test_id('login-email').fill('seed@communitycar.com')
            await page.get_by_test_id('login-password').fill('Password123!')
            await page.get_by_test_id('login-submit').click()
            
            # Wait for navigation or error
            try:
                # Check for settings link presence (hidden but attached) to confirm auth
                await page.wait_for_selector('[data-testid="nav-settings-link"]', state="attached", timeout=3000)
            except:
                # If login failed, try registration flow
                print("Login failed, attempting registration flow...")
                
                # Navigate to register
                await page.goto("http://localhost:5002/register", wait_until="commit")
                
                # Fill registration form
                unique_email = f"test+profile_{asyncio.get_event_loop().time()}@example.com"
                await page.get_by_test_id('register-fullname').fill('Automation User')
                await page.get_by_test_id('register-email').fill(unique_email)
                await page.get_by_test_id('register-password').fill('Password123!')
                await page.get_by_test_id('register-confirm').fill('Password123!')
                await page.get_by_test_id('register-submit').click()
                
                # Check where we are. If redirected to login, log in.
                try:
                    # Wait for either settings link (auto-login) or login email input (redirect to login)
                    await expect(page.get_by_test_id('nav-settings-link').or_(page.get_by_test_id('login-email'))).to_be_visible(timeout=10000)
                    
                    if await page.get_by_test_id('login-email').is_visible():
                        print("Registration redirected to Login. Logging in...")
                        await page.get_by_test_id('login-email').fill(unique_email)
                        await page.get_by_test_id('login-password').fill('Password123!')
                        await page.get_by_test_id('login-submit').click()
                except:
                    pass

                # Check for settings link presence to confirm registration/login
                await page.wait_for_selector('[data-testid="nav-settings-link"]', state="attached", timeout=10000)
        
        except Exception as e:
            with open("debug_output.txt", "w", encoding="utf-8") as f:
                f.write(f"Authentication steps failed: {e}\n")
                f.write(f"Current URL: {page.url}\n")
                try:
                    text = await page.inner_text('body')
                    f.write(f"Page Text: {text}\n")
                    html = await page.content()
                    f.write(f"Page HTML: {html}\n")
                except:
                    pass
            # Fail the test if we can't authenticate
            raise e

        # -> Navigate to Profile Settings
        print("Navigating to Profile Settings...")
        # Open dropdown
        await page.get_by_test_id('nav-profile-toggle').click()
        # Click settings
        await page.get_by_test_id('nav-settings-link').click()
        
        # URL is lowercase in actual app
        await page.wait_for_url("**/profile/settings", timeout=10000)
        
        # -> Update Profile Fields
        print("Updating profile fields...")
        test_city = "Test City " + str(asyncio.get_event_loop().time())
        
        await page.get_by_test_id('profile-fullname-input').fill('Updated Automation Name')
        await page.get_by_test_id('profile-country-input').fill('Test Country')
        await page.get_by_test_id('profile-city-input').fill(test_city)
        await page.get_by_test_id('profile-bio-input').fill('This is an automation test bio.')
        
        # Click Save
        await page.get_by_test_id('profile-save-button').click()
        
        # Allow reload/save
        await page.wait_for_load_state('networkidle')
        
        # -> Verify Update
        print("Verifying update...")
        # Check if the input still has the value (page might reload)
        # Using wait for value
        await expect(page.get_by_test_id('profile-city-input')).to_have_value(test_city)
        
        print("Test Complete!")

    finally:
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

asyncio.run(run_test())
    

import asyncio
from playwright.async_api import async_playwright, expect

async def run_test():
    browser = None
    pw = None
    context = None
    try:
        pw = await async_playwright().start()
        browser = await pw.chromium.launch(headless=True)
        context = await browser.new_context()
        page = await context.new_page()

        # --> TC002-User Registration with Missing Mandatory Fields
        await page.goto("http://localhost:5002/Account/Register")
        
        # Click Create Account with empty fields
        await page.click("#register-button")
        
        # Verify validation messages (localized)
        # These depend on the validation library, usually span classes
        await expect(page.locator("text=Email is required").first).to_be_visible()
        
    finally:
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())

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

        # --> TC011-RTL Layout and Localization Switching
        await page.goto("http://localhost:5002/")
        
        # Look for language toggle
        # It's in the header, usually #language-toggle or text based
        await page.click("#language-dropdown")
        await page.click("text=العربية")
        
        # Verify RTL
        await expect(page.locator("html")).to_have_attribute("dir", "rtl")
        
    finally:
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())

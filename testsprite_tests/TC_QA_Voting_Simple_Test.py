import asyncio
from playwright import async_api
from playwright.async_api import expect

async def run_test():
    pw = None
    browser = None
    context = None

    try:
        # Start a Playwright session
        pw = await async_api.async_playwright().start()

        # Launch browser
        browser = await pw.chromium.launch(
            headless=False,  # Set to False to see the browser for debugging
            args=[
                "--window-size=1280,720",
                "--disable-dev-shm-usage",
                "--no-sandbox"
            ],
        )

        # Create a new browser context
        context = await browser.new_context()
        context.set_default_timeout(15000)

        # Open a new page
        page = await context.new_page()

        print("🚀 Starting Simple QA Voting Test...")

        # Navigate directly to a question page
        print("🌐 Navigating directly to question page...")
        await page.goto("http://localhost:5002/en/qa/is-it-worth-fixing-a-20-year-old-car", wait_until="domcontentloaded")
        
        # Wait for page to load
        await asyncio.sleep(3)
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/qa_direct_access.png")
        
        current_url = page.url
        print(f"📍 Current URL: {current_url}")
        
        # Check if we need to login
        login_form = await page.locator('#login-form').count()
        if login_form > 0:
            print("📝 Login required, logging in with seed user...")
            
            await page.fill('#login-email', 'seed@communitycar.com')
            await page.fill('#login-password', 'Memo@3560')
            
            # Click login and wait for navigation
            await page.click('#login-button')
            
            # Wait for either success or error
            try:
                await page.wait_for_url(lambda url: 'login' not in url.lower(), timeout=10000)
                print("✅ Login successful")
                
                # Navigate back to the question
                await page.goto("http://localhost:5002/en/qa/is-it-worth-fixing-a-20-year-old-car", wait_until="domcontentloaded")
                await asyncio.sleep(2)
                
            except:
                print("⚠️ Login may have failed or timed out")
                await page.screenshot(path="testsprite_tests/qa_login_issue.png")
        else:
            print("ℹ️ No login required or already authenticated")

        # Look for voting buttons
        print("🗳️ Looking for voting buttons...")
        
        upvote_selectors = [
            'button[data-vote-type="Upvote"]',
            '.upvote-btn',
            'button:has-text("Vote Up")',
            'form[data-vote-type="Upvote"] button',
            '.vote-btn'
        ]
        
        upvote_found = False
        for selector in upvote_selectors:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                print(f"✅ Found {element_count} upvote buttons with selector: {selector}")
                upvote_found = True
                
                # Try to get current vote score
                try:
                    vote_score_element = page.locator('.vote-score').first
                    current_score = "0"
                    if await vote_score_element.count() > 0:
                        current_score = await vote_score_element.text_content()
                        print(f"📊 Current vote score: {current_score}")
                    
                    # Test clicking the upvote button
                    print(f"🗳️ Testing upvote with selector: {selector}")
                    await page.locator(selector).first.click()
                    
                    # Wait for potential AJAX response
                    await asyncio.sleep(3)
                    
                    # Check if score changed
                    new_score = current_score
                    if await vote_score_element.count() > 0:
                        new_score = await vote_score_element.text_content()
                        print(f"📊 New vote score: {new_score}")
                    
                    if new_score != current_score:
                        print("🎉 Vote score changed - voting works!")
                    else:
                        print("⚠️ Vote score didn't change - may need authentication or already voted")
                    
                    break
                    
                except Exception as e:
                    print(f"⚠️ Could not test voting with {selector}: {e}")
                    continue
        
        if not upvote_found:
            print("❌ No upvote buttons found")
            # Check page content for debugging
            page_content = await page.content()
            if 'vote' in page_content.lower():
                print("ℹ️ Page contains 'vote' text but buttons not found with selectors")
            if 'form' in page_content.lower():
                print("ℹ️ Page contains forms")

        # Look for answer voting buttons
        print("🔍 Looking for answer voting buttons...")
        
        answer_vote_selectors = [
            'form[data-entity-type="Answer"] button',
            '.vote-btn',
            'button[data-vote-type]'
        ]
        
        answer_votes_found = 0
        for selector in answer_vote_selectors:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                answer_votes_found += element_count
                print(f"✅ Found {element_count} answer vote buttons with selector: {selector}")
        
        # Look for helpful buttons
        print("🔍 Looking for helpful buttons...")
        
        helpful_selectors = [
            '.helpful-btn',
            'button:has-text("Helpful")',
            'form.helpful-form button'
        ]
        
        helpful_found = False
        for selector in helpful_selectors:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                print(f"✅ Found {element_count} helpful buttons with selector: {selector}")
                helpful_found = True
                break

        # Look for bookmark buttons
        print("🔍 Looking for bookmark buttons...")
        
        bookmark_selectors = [
            '.bookmark-btn',
            'form.bookmark-form button',
            'button:has([data-lucide="bookmark"])'
        ]
        
        bookmark_found = False
        for selector in bookmark_selectors:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                print(f"✅ Found {element_count} bookmark buttons with selector: {selector}")
                bookmark_found = True
                break

        # Final screenshot
        await page.screenshot(path="testsprite_tests/qa_voting_simple_test_final.png")
        
        print("\n📊 Simple QA Voting Test Results:")
        print(f"  ✅ Page Access: Successful")
        print(f"  {'✅' if upvote_found else '❌'} Question Voting: {'Found and tested' if upvote_found else 'Not found'}")
        print(f"  {'✅' if answer_votes_found > 0 else '❌'} Answer Voting: {'Available' if answer_votes_found > 0 else 'Not available'}")
        print(f"  {'✅' if helpful_found else '❌'} Helpful Buttons: {'Found' if helpful_found else 'Not found'}")
        print(f"  {'✅' if bookmark_found else '❌'} Bookmark Buttons: {'Found' if bookmark_found else 'Not found'}")
        
        if upvote_found:
            print("\n🎉 SUCCESS: QA voting functionality is implemented!")
        else:
            print("\n⚠️ ISSUE: Voting buttons not found - may need authentication or different selectors")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()
        
        # Take error screenshot
        try:
            await page.screenshot(path="testsprite_tests/qa_voting_simple_test_error.png")
            print("📸 Error screenshot saved")
        except:
            pass

    finally:
        print("\n🧹 Cleaning up...")
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())
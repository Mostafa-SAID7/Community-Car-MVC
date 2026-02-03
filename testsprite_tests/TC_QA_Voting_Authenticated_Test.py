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

        print("🚀 Starting Authenticated QA Voting Test...")

        # Navigate to login page first
        print("🌐 Navigating to login page...")
        await page.goto("http://localhost:5002/account/login", wait_until="domcontentloaded")
        
        # Wait for page to load
        await asyncio.sleep(2)
        
        # Login with seed user
        print("📝 Logging in with seed user...")
        await page.fill('#login-email', 'seed@communitycar.com')
        await page.fill('#login-password', 'Memo@3560')
        
        # Submit login form
        await page.click('#login-button')
        
        # Wait for login to complete
        try:
            await page.wait_for_url(lambda url: 'login' not in url.lower(), timeout=10000)
            print("✅ Login successful")
        except:
            print("⚠️ Login timeout - checking current page...")
            current_url = page.url
            if 'login' not in current_url.lower():
                print("✅ Login appears successful (not on login page)")
            else:
                print("❌ Still on login page")
                await page.screenshot(path="testsprite_tests/qa_auth_login_failed.png")
                return

        # Navigate to QA question
        print("🔧 Navigating to QA question...")
        await page.goto("http://localhost:5002/en/qa/is-it-worth-fixing-a-20-year-old-car", wait_until="domcontentloaded")
        await asyncio.sleep(3)
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/qa_auth_question_page.png")
        
        print(f"📍 Current URL: {page.url}")

        # Test question voting
        print("🗳️ Testing question voting...")
        
        # Get current vote score
        vote_score_element = page.locator('.vote-score').first
        current_score = "0"
        if await vote_score_element.count() > 0:
            current_score = await vote_score_element.text_content()
            print(f"📊 Current question vote score: {current_score}")

        # Click upvote button
        upvote_button = page.locator('form[data-entity-type="Question"][data-vote-type="Upvote"] button').first
        if await upvote_button.count() > 0:
            print("🗳️ Clicking question upvote button...")
            await upvote_button.click()
            
            # Wait for AJAX response
            await asyncio.sleep(3)
            
            # Check new score
            new_score = current_score
            if await vote_score_element.count() > 0:
                new_score = await vote_score_element.text_content()
                print(f"📊 New question vote score: {new_score}")
            
            if new_score != current_score:
                print("🎉 Question voting works! Score changed.")
                question_voting_works = True
            else:
                print("⚠️ Question vote score didn't change")
                question_voting_works = False
        else:
            print("❌ Question upvote button not found")
            question_voting_works = False

        # Test answer voting if answers exist
        print("🗳️ Testing answer voting...")
        
        answer_upvote_buttons = page.locator('form[data-entity-type="Answer"][data-vote-type="Upvote"] button')
        answer_voting_works = False
        
        if await answer_upvote_buttons.count() > 0:
            print(f"✅ Found {await answer_upvote_buttons.count()} answer upvote buttons")
            
            # Get first answer's vote score
            first_answer_score = page.locator('.vote-score').nth(1)  # Second vote score (first is question)
            if await first_answer_score.count() > 0:
                current_answer_score = await first_answer_score.text_content()
                print(f"📊 Current answer vote score: {current_answer_score}")
                
                # Click first answer upvote
                await answer_upvote_buttons.first.click()
                await asyncio.sleep(3)
                
                # Check new score
                new_answer_score = await first_answer_score.text_content()
                print(f"📊 New answer vote score: {new_answer_score}")
                
                if new_answer_score != current_answer_score:
                    print("🎉 Answer voting works! Score changed.")
                    answer_voting_works = True
                else:
                    print("⚠️ Answer vote score didn't change")
        else:
            print("ℹ️ No answers found to test voting")

        # Test helpful button
        print("👍 Testing helpful button...")
        
        helpful_button = page.locator('.helpful-btn').first
        helpful_works = False
        
        if await helpful_button.count() > 0:
            # Get current helpful count
            helpful_count_element = helpful_button.locator('.helpful-count')
            current_helpful_count = "0"
            if await helpful_count_element.count() > 0:
                current_helpful_count = await helpful_count_element.text_content()
                print(f"📊 Current helpful count: {current_helpful_count}")
            
            # Click helpful button
            await helpful_button.click()
            await asyncio.sleep(3)
            
            # Check new count
            if await helpful_count_element.count() > 0:
                new_helpful_count = await helpful_count_element.text_content()
                print(f"📊 New helpful count: {new_helpful_count}")
                
                if new_helpful_count != current_helpful_count:
                    print("🎉 Helpful button works! Count changed.")
                    helpful_works = True
                else:
                    print("⚠️ Helpful count didn't change")
        else:
            print("❌ Helpful button not found")

        # Test bookmark button
        print("🔖 Testing bookmark button...")
        
        bookmark_button = page.locator('.bookmark-btn').first
        bookmark_works = False
        
        if await bookmark_button.count() > 0:
            # Check current bookmark state
            is_bookmarked_before = await bookmark_button.get_attribute('class')
            has_amber_before = 'text-amber-500' in (is_bookmarked_before or '')
            print(f"📊 Bookmarked before: {has_amber_before}")
            
            # Click bookmark button
            await bookmark_button.click()
            await asyncio.sleep(3)
            
            # Check new state
            is_bookmarked_after = await bookmark_button.get_attribute('class')
            has_amber_after = 'text-amber-500' in (is_bookmarked_after or '')
            print(f"📊 Bookmarked after: {has_amber_after}")
            
            if has_amber_before != has_amber_after:
                print("🎉 Bookmark button works! State changed.")
                bookmark_works = True
            else:
                print("⚠️ Bookmark state didn't change")
        else:
            print("❌ Bookmark button not found")

        # Final screenshot
        await page.screenshot(path="testsprite_tests/qa_auth_test_final.png")
        
        print("\n📊 Authenticated QA Voting Test Results:")
        print(f"  ✅ Authentication: Successful")
        print(f"  {'✅' if question_voting_works else '❌'} Question Voting: {'Working' if question_voting_works else 'Not working'}")
        print(f"  {'✅' if answer_voting_works else '❌'} Answer Voting: {'Working' if answer_voting_works else 'Not working'}")
        print(f"  {'✅' if helpful_works else '❌'} Helpful Button: {'Working' if helpful_works else 'Not working'}")
        print(f"  {'✅' if bookmark_works else '❌'} Bookmark Button: {'Working' if bookmark_works else 'Not working'}")
        
        total_working = sum([question_voting_works, answer_voting_works, helpful_works, bookmark_works])
        
        if total_working >= 3:
            print(f"\n🎉 SUCCESS: {total_working}/4 QA features are working!")
        elif total_working >= 2:
            print(f"\n⚠️ PARTIAL SUCCESS: {total_working}/4 QA features are working")
        else:
            print(f"\n❌ ISSUES: Only {total_working}/4 QA features are working")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()
        
        # Take error screenshot
        try:
            await page.screenshot(path="testsprite_tests/qa_auth_test_error.png")
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
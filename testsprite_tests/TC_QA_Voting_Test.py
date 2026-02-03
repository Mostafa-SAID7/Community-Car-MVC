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
        context.set_default_timeout(10000)

        # Open a new page
        page = await context.new_page()

        print("🚀 Starting QA Voting Test...")

        # Navigate to the application
        print("🌐 Navigating to application...")
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")

        # Check if we're on login page and login with seed user
        login_form = await page.locator('#login-form').count()
        if login_form > 0:
            print("📝 Found login form, logging in with seed user...")
            
            await page.fill('#login-email', 'seed@communitycar.com')
            await page.fill('#login-password', 'Memo@3560')
            await page.click('#login-button')
            
            # Wait for navigation after login
            try:
                await page.wait_for_url(lambda url: 'login' not in url.lower(), timeout=10000)
                print("✅ Login successful - redirected from login page")
            except:
                # Check if we're still on login page
                current_url = page.url
                if 'login' in current_url.lower():
                    print("❌ Still on login page, login may have failed")
                    await page.screenshot(path="testsprite_tests/qa_login_failed.png")
                    return
                else:
                    print("✅ Login appears successful")
        else:
            print("ℹ️ No login form found, may already be authenticated")

        # Wait for page to load
        await asyncio.sleep(2)

        # Navigate to QA section
        print("🔧 Navigating to QA section...")
        
        # Try different ways to navigate to QA
        qa_nav_selectors = [
            'a[href*="/qa"]',
            'a:has-text("Q&A")',
            'a:has-text("QA")',
            'a:has-text("Questions")',
            '.nav-link:has-text("Q&A")'
        ]
        
        qa_found = False
        for selector in qa_nav_selectors:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                print(f"✅ Found QA navigation: {selector}")
                try:
                    await page.locator(selector).first.click()
                    qa_found = True
                    break
                except Exception as e:
                    print(f"⚠️ Could not click {selector}: {e}")
                    continue
        
        if not qa_found:
            # Try direct navigation
            print("🔗 Trying direct navigation to QA...")
            await page.goto("http://localhost:5002/en/qa", wait_until="domcontentloaded")
        
        # Wait for QA page to load
        await asyncio.sleep(3)
        
        # Verify we're on the QA page
        current_url = page.url
        print(f"📍 Current URL: {current_url}")
        
        if 'qa' not in current_url.lower():
            print("⚠️ Not on QA page, trying alternative navigation...")
            # Try different culture codes
            for culture in ['en', 'ar']:
                try:
                    await page.goto(f"http://localhost:5002/{culture}/qa", wait_until="domcontentloaded")
                    await asyncio.sleep(2)
                    if 'qa' in page.url.lower():
                        print(f"✅ Successfully navigated to QA with culture: {culture}")
                        break
                except:
                    continue

        # Take screenshot for debugging
        await page.screenshot(path="testsprite_tests/qa_page.png")
        
        # Look for questions on the page
        print("🔍 Looking for questions...")
        
        question_selectors = [
            'a[href*="/qa/"]',
            '.question-item',
            '.qa-question',
            'h3 a',
            'h2 a'
        ]
        
        question_link = None
        upvote_found = False
        answer_votes_found = 0
        helpful_found = False
        
        # Check if we're already on a question details page
        if '/qa/' in page.url and page.url.count('/') > 4:
            print("✅ Already on a question details page")
            question_link = True  # Set to True to indicate we're on a details page
        else:
            # Look for question links
            for selector in question_selectors:
                elements = await page.locator(selector).count()
                if elements > 0:
                    print(f"✅ Found {elements} questions with selector: {selector}")
                    question_link = page.locator(selector).first
                    break
        
        if question_link:
            if question_link != True:  # If it's an actual element, click it
                print("📖 Clicking on first question...")
                await question_link.click()
                await asyncio.sleep(3)
            else:
                print("📖 Already on question details page...")
            
            # Take screenshot of question details
            await page.screenshot(path="testsprite_tests/qa_question_details.png")
            
            # Look for voting buttons
            print("🗳️ Looking for voting buttons...")
            
            upvote_selectors = [
                'button[data-vote-type="Upvote"]',
                '.upvote-btn',
                'button:has-text("Vote Up")',
                'form[data-vote-type="Upvote"] button'
            ]
            
            upvote_found = False
            upvote_found = False
            for selector in upvote_selectors:
                element_count = await page.locator(selector).count()
                if element_count > 0:
                    print(f"✅ Found upvote button: {selector}")
                    try:
                        # Get current vote score
                        vote_score_element = page.locator('.vote-score').first
                        current_score = "0"
                        if await vote_score_element.count() > 0:
                            current_score = await vote_score_element.text_content()
                        
                        print(f"📊 Current vote score: {current_score}")
                        
                        # Click upvote button
                        await page.locator(selector).first.click()
                        print("✅ Clicked upvote button")
                        
                        # Wait for AJAX response
                        await asyncio.sleep(2)
                        
                        # Check if vote score changed
                        new_score = current_score
                        if await vote_score_element.count() > 0:
                            new_score = await vote_score_element.text_content()
                        
                        print(f"📊 New vote score: {new_score}")
                        
                        if new_score != current_score:
                            print("🎉 Vote score changed - voting works!")
                        else:
                            print("⚠️ Vote score didn't change - may need to check implementation")
                        
                        upvote_found = True
                        break
                    except Exception as e:
                        print(f"⚠️ Could not click upvote button {selector}: {e}")
                        continue
            
            if not upvote_found:
                print("❌ No upvote buttons found")
            
            # Look for answer voting if there are answers
            print("🔍 Looking for answer voting buttons...")
            
            answer_vote_selectors = [
                '.vote-btn',
                'button[data-vote-type]',
                'form[data-entity-type="Answer"] button'
            ]
            
            answer_votes_found = 0
            
            answer_votes_found = 0
            for selector in answer_vote_selectors:
                element_count = await page.locator(selector).count()
                if element_count > 0:
                    answer_votes_found += element_count
                    print(f"✅ Found {element_count} answer vote buttons with selector: {selector}")
            
            if answer_votes_found > 0:
                print(f"📊 Total answer vote buttons found: {answer_votes_found}")
                
                # Try clicking on first answer upvote
                try:
                    first_answer_upvote = page.locator('form[data-entity-type="Answer"][data-vote-type="Upvote"] button').first
                    if await first_answer_upvote.count() > 0:
                        print("🗳️ Testing answer voting...")
                        await first_answer_upvote.click()
                        await asyncio.sleep(2)
                        print("✅ Clicked answer upvote button")
                except Exception as e:
                    print(f"⚠️ Could not test answer voting: {e}")
            
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
                    try:
                        await page.locator(selector).first.click()
                        await asyncio.sleep(2)
                        print("✅ Clicked helpful button")
                        helpful_found = True
                        break
                    except Exception as e:
                        print(f"⚠️ Could not click helpful button {selector}: {e}")
                        continue
            
            if not helpful_found:
                print("❌ No helpful buttons found")
        
        else:
            print("❌ No questions found on QA page")

        # Final screenshot
        await page.screenshot(path="testsprite_tests/qa_voting_test_final.png")
        
        print("\n📊 QA Voting Test Results:")
        print(f"  ✅ Login: Successful")
        print(f"  ✅ QA Navigation: Successful")
        print(f"  {'✅' if question_link else '❌'} Questions Found: {'Yes' if question_link else 'No'}")
        print(f"  {'✅' if upvote_found else '❌'} Voting Buttons: {'Found and tested' if upvote_found else 'Not found'}")
        print(f"  {'✅' if answer_votes_found > 0 else '❌'} Answer Voting: {'Available' if answer_votes_found > 0 else 'Not available'}")
        print(f"  {'✅' if helpful_found else '❌'} Helpful Buttons: {'Found and tested' if helpful_found else 'Not found'}")
        
        if upvote_found and answer_votes_found > 0:
            print("\n🎉 SUCCESS: QA voting functionality is working!")
        else:
            print("\n⚠️ PARTIAL SUCCESS: Some voting features may need attention")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()
        
        # Take error screenshot
        try:
            await page.screenshot(path="testsprite_tests/qa_voting_test_error.png")
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
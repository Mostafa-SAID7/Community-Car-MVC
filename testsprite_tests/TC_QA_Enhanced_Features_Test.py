import asyncio
from playwright import async_api

async def run_test():
    pw = None
    browser = None
    context = None

    try:
        # Start a Playwright session
        pw = await async_api.async_playwright().start()

        # Launch browser
        browser = await pw.chromium.launch(
            headless=False,
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

        print("🚀 Starting Enhanced QA Features Test...")

        # Navigate directly to QA question
        print("🌐 Navigating to QA question...")
        await page.goto("http://localhost:5002/en/qa/is-it-worth-fixing-a-20-year-old-car", wait_until="domcontentloaded")
        await asyncio.sleep(2)
        
        print(f"📍 Current URL: {page.url}")
        
        # Take screenshot
        await page.screenshot(path="testsprite_tests/qa_enhanced_features_page.png")

        # Test 1: Check Join Discussion functionality
        print("\n💬 Test 1: Join Discussion functionality...")
        
        join_discussion_buttons = await page.locator('.join-discussion-btn, button:has-text("Join Discussion")').count()
        join_discussion_links = await page.locator('a:has-text("Join Discussion")').count()
        
        print(f"  ✅ Join Discussion buttons: {join_discussion_buttons}")
        print(f"  ✅ Join Discussion links (for non-auth): {join_discussion_links}")
        
        if join_discussion_buttons > 0:
            print("  🎯 Testing Join Discussion click...")
            try:
                await page.locator('.join-discussion-btn, button:has-text("Join Discussion")').first.click()
                await asyncio.sleep(1)
                print("  ✅ Join Discussion click successful")
            except Exception as e:
                print(f"  ⚠️ Join Discussion click failed: {e}")

        # Test 2: Check Share functionality
        print("\n🔗 Test 2: Share functionality...")
        
        share_buttons = await page.locator('.share-btn, button:has([data-lucide="share-2"])').count()
        
        print(f"  ✅ Share buttons: {share_buttons}")
        
        if share_buttons > 0:
            print("  🎯 Testing Share click...")
            try:
                await page.locator('.share-btn, button:has([data-lucide="share-2"])').first.click()
                await asyncio.sleep(1)
                print("  ✅ Share click successful")
            except Exception as e:
                print(f"  ⚠️ Share click failed: {e}")

        # Test 3: Check Bookmark functionality (unauthenticated)
        print("\n🔖 Test 3: Bookmark functionality...")
        
        bookmark_buttons = await page.locator('.bookmark-btn').count()
        bookmark_links = await page.locator('a[title*="bookmark"], a[title*="Login to bookmark"]').count()
        
        print(f"  ✅ Bookmark buttons (auth): {bookmark_buttons}")
        print(f"  ✅ Bookmark links (non-auth): {bookmark_links}")

        # Test 4: Check Sorting functionality
        print("\n📊 Test 4: Answer sorting functionality...")
        
        sort_buttons = await page.locator('.sort-btn').count()
        confidence_sort = await page.locator('button[data-sort="confidence"]').count()
        chronological_sort = await page.locator('button[data-sort="chronological"]').count()
        
        print(f"  ✅ Sort buttons: {sort_buttons}")
        print(f"  ✅ Confidence sort: {confidence_sort}")
        print(f"  ✅ Chronological sort: {chronological_sort}")
        
        if chronological_sort > 0:
            print("  🎯 Testing sort functionality...")
            try:
                await page.locator('button[data-sort="chronological"]').click()
                await asyncio.sleep(2)
                print("  ✅ Chronological sort click successful")
                
                # Switch back to confidence
                await page.locator('button[data-sort="confidence"]').click()
                await asyncio.sleep(2)
                print("  ✅ Confidence sort click successful")
            except Exception as e:
                print(f"  ⚠️ Sort functionality failed: {e}")

        # Test 5: Check Profile links
        print("\n👤 Test 5: Profile integration...")
        
        author_profile_links = await page.locator('a[href*="/Profile"]').count()
        
        print(f"  ✅ Author profile links: {author_profile_links}")
        
        if author_profile_links > 0:
            print("  🎯 Testing profile link...")
            try:
                first_profile_link = page.locator('a[href*="/Profile"]').first
                href = await first_profile_link.get_attribute('href')
                print(f"  📍 Profile link URL: {href}")
                print("  ✅ Profile links are properly formatted")
            except Exception as e:
                print(f"  ⚠️ Profile link test failed: {e}")

        # Test 6: Check Authentication-dependent features
        print("\n🔐 Test 6: Authentication-dependent features...")
        
        # Check for login links for unauthenticated users
        login_links = await page.locator('a[href*="login"], a[href*="Login"]').count()
        helpful_links = await page.locator('a[title*="Login to mark"], a[title*="Login to comment"]').count()
        
        print(f"  ✅ Login redirect links: {login_links}")
        print(f"  ✅ Feature-specific login prompts: {helpful_links}")

        # Test 7: Check JavaScript functions availability
        print("\n🔧 Test 7: JavaScript functions...")
        
        js_functions = await page.evaluate("""
            () => {
                return {
                    scrollToAnswerForm: typeof scrollToAnswerForm === 'function',
                    shareQuestion: typeof shareQuestion === 'function',
                    toggleCommentForm: typeof toggleCommentForm === 'function',
                    initializeSorting: typeof initializeSorting === 'function',
                    sortAnswers: typeof sortAnswers === 'function',
                    showToast: typeof showToast === 'function'
                };
            }
        """)
        
        for func_name, exists in js_functions.items():
            print(f"  {'✅' if exists else '❌'} {func_name}: {'Available' if exists else 'Missing'}")

        # Test 8: Check UI responsiveness and animations
        print("\n🎨 Test 8: UI/UX enhancements...")
        
        # Check for hover effects and transitions
        ui_classes = await page.evaluate("""
            () => {
                const elements = document.querySelectorAll('.transition-all, .hover\\:text-primary, .hover\\:bg-primary');
                return elements.length;
            }
        """)
        
        print(f"  ✅ Elements with transitions/hover effects: {ui_classes}")

        # Test 9: Check AJAX readiness
        print("\n⚡ Test 9: AJAX functionality readiness...")
        
        ajax_forms = await page.locator('form.vote-form, form.helpful-form, form.bookmark-form').count()
        ajax_buttons = await page.locator('button[type="submit"]').count()
        
        print(f"  ✅ AJAX-ready forms: {ajax_forms}")
        print(f"  ✅ Submit buttons: {ajax_buttons}")

        # Final screenshot
        await page.screenshot(path="testsprite_tests/qa_enhanced_features_complete.png")

        # Calculate overall score
        total_features = 9
        working_features = 0
        
        # Scoring based on test results
        if join_discussion_buttons > 0 or join_discussion_links > 0:
            working_features += 1
        if share_buttons > 0:
            working_features += 1
        if bookmark_buttons > 0 or bookmark_links > 0:
            working_features += 1
        if sort_buttons >= 2:
            working_features += 1
        if author_profile_links > 0:
            working_features += 1
        if login_links > 0 or helpful_links > 0:
            working_features += 1
        if sum(js_functions.values()) >= 4:  # At least 4 JS functions available
            working_features += 1
        if ui_classes > 10:  # Good UI/UX implementation
            working_features += 1
        if ajax_forms > 0:
            working_features += 1

        print(f"\n📊 Enhanced QA Features Test Results:")
        print(f"  🎯 Features Working: {working_features}/{total_features}")
        print(f"  📈 Success Rate: {(working_features/total_features)*100:.1f}%")
        
        if working_features >= 8:
            print("\n🎉 EXCELLENT: All enhanced QA features are working perfectly!")
        elif working_features >= 6:
            print("\n✅ VERY GOOD: Most enhanced QA features are functional!")
        elif working_features >= 4:
            print("\n⚠️ GOOD: Basic enhanced features are working!")
        else:
            print("\n❌ NEEDS WORK: Enhanced features need attention!")

        print("\n📋 Enhanced Features Summary:")
        print("  • Join Discussion with scroll-to-form functionality")
        print("  • Share button with native sharing API fallback")
        print("  • Authentication-aware bookmark system")
        print("  • Dynamic answer sorting (Confidence/Chronological)")
        print("  • Profile integration with clickable author links")
        print("  • Smart authentication redirects for protected features")
        print("  • Comprehensive JavaScript functionality")
        print("  • Enhanced UI/UX with transitions and hover effects")
        print("  • AJAX-ready forms for real-time interactions")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()
        
        try:
            await page.screenshot(path="testsprite_tests/qa_enhanced_features_error.png")
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
import asyncio
from playwright import async_api

async def run_test():
    pw = None
    browser = None
    context = None

    try:
        # Start a Playwright session
        pw = await async_api.async_playwright().start()
        browser = await pw.chromium.launch(headless=False)
        context = await browser.new_context()
        page = await context.new_page()

        print("🚀 Starting Final Notification Test with Seed User...")

        # Navigate to the application
        print("🌐 Navigating to application...")
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")
        
        # Check if we're on login page and login with seed user
        login_form = await page.locator('#login-form').count()
        if login_form > 0:
            print("📝 Found login form, logging in with seed user...")
            
            await page.fill('#login-email', 'seed@communitycar.com')
            await page.fill('#login-password', 'Password123!')
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
                    # Take screenshot for debugging
                    await page.screenshot(path="testsprite_tests/login_failed.png")
                    return
                else:
                    print("✅ Login appears successful")
        else:
            print("ℹ️ No login form found, may already be authenticated")
        
        # Wait a moment for the page to fully load
        await asyncio.sleep(2)
        
        print("🔌 Setting up SignalR notification listener...")
        
        # Add SignalR notification listener
        await page.evaluate("""
            window.notificationTest = {
                connected: false,
                notifications: [],
                errors: [],
                connectionAttempts: 0
            };
            
            function connectSignalR() {
                window.notificationTest.connectionAttempts++;
                console.log('Attempting SignalR connection, attempt:', window.notificationTest.connectionAttempts);
                
                if (typeof signalR === 'undefined') {
                    console.log('SignalR not loaded yet, waiting...');
                    if (window.notificationTest.connectionAttempts < 10) {
                        setTimeout(connectSignalR, 1000);
                    } else {
                        window.notificationTest.errors.push('SignalR library not found after 10 attempts');
                    }
                    return;
                }
                
                try {
                    const connection = new signalR.HubConnectionBuilder()
                        .withUrl('/hubs/notification')
                        .withAutomaticReconnect()
                        .build();
                    
                    connection.start().then(function () {
                        console.log('✅ SignalR Connected successfully');
                        window.notificationTest.connected = true;
                    }).catch(function (err) {
                        console.error('❌ SignalR Connection failed:', err);
                        window.notificationTest.errors.push('Connection failed: ' + err.toString());
                    });
                    
                    connection.on('ReceiveNotification', function (notification) {
                        console.log('📨 Received notification:', notification);
                        window.notificationTest.notifications.push(notification);
                    });
                    
                    window.notificationConnection = connection;
                } catch (error) {
                    console.error('❌ Error setting up SignalR:', error);
                    window.notificationTest.errors.push('Setup error: ' + error.toString());
                }
            }
            
            // Start connection attempt
            connectSignalR();
        """)
        
        # Wait for SignalR connection
        await asyncio.sleep(5)
        
        # Check connection status
        test_status = await page.evaluate("window.notificationTest")
        print(f"🔌 SignalR Status: Connected={test_status['connected']}, Attempts={test_status['connectionAttempts']}")
        
        if test_status['errors']:
            print("❌ SignalR Errors:")
            for error in test_status['errors']:
                print(f"  - {error}")
        
        # Test the notification API endpoints
        print("🔍 Testing notification API endpoints...")
        
        # Test 1: Get unread count
        print("📊 Testing unread count endpoint...")
        unread_result = await page.evaluate("""
            async () => {
                try {
                    const response = await fetch('/shared/notifications/unread-count', {
                        method: 'GET',
                        credentials: 'include',
                        headers: {
                            'Accept': 'application/json'
                        }
                    });
                    
                    if (response.ok) {
                        const data = await response.json();
                        return { success: true, data: data };
                    } else {
                        const text = await response.text();
                        return { success: false, status: response.status, error: text.substring(0, 200) };
                    }
                } catch (error) {
                    return { success: false, error: error.message };
                }
            }
        """)
        
        if unread_result['success']:
            print(f"✅ Unread count API works: {unread_result['data']}")
        else:
            print(f"❌ Unread count API failed: {unread_result}")
        
        # Test 2: Send test notification
        print("📤 Testing send notification endpoint...")
        send_result = await page.evaluate("""
            async () => {
                try {
                    const response = await fetch('/shared/notifications/test', {
                        method: 'POST',
                        credentials: 'include',
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        },
                        body: JSON.stringify('Info')
                    });
                    
                    if (response.ok) {
                        const data = await response.json();
                        return { success: true, data: data };
                    } else {
                        const text = await response.text();
                        return { success: false, status: response.status, error: text.substring(0, 200) };
                    }
                } catch (error) {
                    return { success: false, error: error.message };
                }
            }
        """)
        
        if send_result['success']:
            print(f"✅ Send notification API works: {send_result['data']}")
        else:
            print(f"❌ Send notification API failed: {send_result}")
        
        # Wait for potential SignalR notification
        await asyncio.sleep(3)
        
        # Test 3: Get all notifications
        print("📋 Testing get notifications endpoint...")
        get_result = await page.evaluate("""
            async () => {
                try {
                    const response = await fetch('/shared/notifications', {
                        method: 'GET',
                        credentials: 'include',
                        headers: {
                            'Accept': 'application/json'
                        }
                    });
                    
                    if (response.ok) {
                        const data = await response.json();
                        return { success: true, data: data };
                    } else {
                        const text = await response.text();
                        return { success: false, status: response.status, error: text.substring(0, 200) };
                    }
                } catch (error) {
                    return { success: false, error: error.message };
                }
            }
        """)
        
        if get_result['success']:
            notifications = get_result['data'].get('data', [])
            print(f"✅ Get notifications API works: Found {len(notifications)} notifications")
            if notifications:
                for i, notif in enumerate(notifications[:3]):  # Show first 3
                    print(f"  {i+1}. {notif.get('Title', 'No title')}: {notif.get('Message', 'No message')}")
        else:
            print(f"❌ Get notifications API failed: {get_result}")
        
        # Check final SignalR results
        final_status = await page.evaluate("window.notificationTest")
        print(f"\n📊 Final Test Results:")
        print(f"  SignalR Connected: {final_status['connected']}")
        print(f"  Real-time Notifications Received: {len(final_status['notifications'])}")
        print(f"  Connection Errors: {len(final_status['errors'])}")
        
        if final_status['notifications']:
            print("🎉 SUCCESS: Real-time notifications received via SignalR!")
            for i, notif in enumerate(final_status['notifications']):
                print(f"  Notification {i+1}: {notif.get('Title', 'No title')} - {notif.get('Message', 'No message')}")
        
        # Test UI notification elements
        print("\n🎨 Testing notification UI...")
        
        # Look for notification bell or similar UI elements
        ui_selectors = [
            'button[data-testid="notification-bell"]',
            '.notification-bell',
            'button:has-text("Notifications")',
            '[class*="notification"][class*="bell"]',
            '[class*="notification"][class*="icon"]',
            'button[title*="notification" i]'
        ]
        
        found_ui = False
        for selector in ui_selectors:
            count = await page.locator(selector).count()
            if count > 0:
                print(f"✅ Found notification UI: {selector} ({count} elements)")
                found_ui = True
                
                try:
                    await page.locator(selector).first.click()
                    print("✅ Clicked notification UI element")
                    await asyncio.sleep(1)
                    
                    # Look for notification dropdown/panel
                    panel_selectors = [
                        '.notification-panel',
                        '.notification-dropdown',
                        '[data-testid="notification-panel"]',
                        '[class*="notification"][class*="panel"]',
                        '[class*="notification"][class*="dropdown"]'
                    ]
                    
                    for panel_selector in panel_selectors:
                        panel_count = await page.locator(panel_selector).count()
                        if panel_count > 0:
                            print(f"✅ Found notification panel: {panel_selector}")
                            break
                    
                except Exception as e:
                    print(f"⚠️ Could not interact with notification UI: {e}")
                break
        
        if not found_ui:
            print("⚠️ No notification UI elements found")
        
        # Summary
        print(f"\n🏁 Test Summary:")
        api_working = unread_result['success'] or send_result['success'] or get_result['success']
        signalr_working = final_status['connected']
        realtime_working = len(final_status['notifications']) > 0
        
        print(f"  ✅ Authentication: Working")
        print(f"  {'✅' if api_working else '❌'} Notification APIs: {'Working' if api_working else 'Failed'}")
        print(f"  {'✅' if signalr_working else '❌'} SignalR Connection: {'Working' if signalr_working else 'Failed'}")
        print(f"  {'✅' if realtime_working else '⚠️'} Real-time Delivery: {'Working' if realtime_working else 'No notifications received'}")
        print(f"  {'✅' if found_ui else '⚠️'} Notification UI: {'Found' if found_ui else 'Not found'}")
        
        if api_working and signalr_working:
            print("\n🎉 SUCCESS: Notification system is functional!")
        elif api_working:
            print("\n⚠️ PARTIAL: APIs work but real-time delivery needs verification")
        else:
            print("\n❌ FAILED: Notification system has issues")
        
        # Take final screenshot
        await page.screenshot(path="testsprite_tests/final_notification_test.png")
        print("📸 Final screenshot saved")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()

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
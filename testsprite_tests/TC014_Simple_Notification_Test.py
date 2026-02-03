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

        print("🚀 Starting Simple Notification Test...")

        # Navigate to the application
        print("🌐 Navigating to application...")
        await page.goto("http://localhost:5002", wait_until="domcontentloaded")
        
        # Check if we're on login page
        login_form = await page.locator('#login-form').count()
        if login_form > 0:
            print("📝 Found login form, attempting to log in...")
            
            # Try to login with seed user
            await page.fill('#login-email', 'seed@communitycar.com')
            await page.fill('#login-password', 'Password123!')
            await page.click('#login-button')
            await page.wait_for_timeout(3000)
            
            # Check if login was successful
            current_url = page.url
            if 'login' not in current_url.lower():
                print("✅ Login successful")
            else:
                print("❌ Login failed, trying registration...")
                
                # Try to register a new user
                await page.goto("http://localhost:5002/register", wait_until="domcontentloaded")
                await page.fill('#register-fullname', 'Test User')
                await page.fill('#register-email', 'testuser@example.com')
                await page.fill('#register-password', 'Password123!')
                await page.fill('#register-confirm', 'Password123!')
                await page.click('#register-button')
                await page.wait_for_timeout(3000)
                print("✅ Registration completed")
        
        # Set up notification listener
        print("🔌 Setting up notification listener...")
        await page.add_init_script("""
            window.notificationResults = {
                connected: false,
                notifications: [],
                errors: []
            };
            
            // Wait for SignalR to be available
            function setupNotifications() {
                if (typeof signalR !== 'undefined') {
                    try {
                        const connection = new signalR.HubConnectionBuilder()
                            .withUrl('/hubs/notification')
                            .build();
                        
                        connection.start().then(function () {
                            console.log('SignalR Connected for notifications');
                            window.notificationResults.connected = true;
                        }).catch(function (err) {
                            console.error('SignalR Connection Error: ', err.toString());
                            window.notificationResults.errors.push(err.toString());
                        });
                        
                        connection.on('ReceiveNotification', function (notification) {
                            console.log('Received notification:', notification);
                            window.notificationResults.notifications.push(notification);
                        });
                        
                        window.notificationConnection = connection;
                    } catch (error) {
                        console.error('Error setting up SignalR:', error);
                        window.notificationResults.errors.push(error.toString());
                    }
                } else {
                    console.log('SignalR not available, retrying in 1 second...');
                    setTimeout(setupNotifications, 1000);
                }
            }
            
            // Start setup when DOM is ready
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', setupNotifications);
            } else {
                setupNotifications();
            }
        """)
        
        # Reload page to ensure script is executed
        await page.reload(wait_until="domcontentloaded")
        await asyncio.sleep(3)
        
        # Check SignalR connection status
        connection_status = await page.evaluate("window.notificationResults")
        print(f"🔌 SignalR Connection Status: {connection_status}")
        
        if connection_status.get('connected'):
            print("✅ SignalR connected successfully")
        else:
            print("❌ SignalR connection failed")
            if connection_status.get('errors'):
                for error in connection_status['errors']:
                    print(f"  Error: {error}")
        
        # Test notification UI elements
        print("🎨 Testing notification UI elements...")
        
        # Look for notification elements in the page
        notification_elements = [
            'button[data-testid="notification-bell"]',
            '.notification-bell',
            'button:has-text("Notifications")',
            '[class*="notification"]',
            '[id*="notification"]',
            'button[title*="notification"]',
            'button[title*="Notification"]'
        ]
        
        found_notification_ui = False
        for selector in notification_elements:
            element_count = await page.locator(selector).count()
            if element_count > 0:
                print(f"✅ Found notification UI element: {selector} ({element_count} elements)")
                found_notification_ui = True
                
                # Try to click the first one
                try:
                    await page.locator(selector).first.click()
                    print("✅ Clicked notification element")
                    await asyncio.sleep(1)
                except Exception as e:
                    print(f"⚠️ Could not click notification element: {e}")
                break
        
        if not found_notification_ui:
            print("⚠️ No notification UI elements found")
        
        # Test API endpoints (if authenticated)
        print("🔍 Testing notification API endpoints...")
        
        # Test unread count endpoint
        try:
            unread_response = await page.evaluate("""
                async () => {
                    try {
                        const response = await fetch('/shared/notifications/unread-count', {
                            method: 'GET',
                            credentials: 'include'
                        });
                        const text = await response.text();
                        try {
                            return JSON.parse(text);
                        } catch {
                            return { error: 'Not JSON', status: response.status, text: text.substring(0, 100) };
                        }
                    } catch (error) {
                        return { error: error.message };
                    }
                }
            """)
            
            if unread_response.get('success'):
                print(f"✅ Unread count API works: {unread_response.get('data', 0)} unread notifications")
            else:
                print(f"⚠️ Unread count API response: {unread_response}")
        except Exception as e:
            print(f"❌ Failed to test unread count API: {e}")
        
        # Test sending a test notification
        try:
            test_notification_response = await page.evaluate("""
                async () => {
                    try {
                        const response = await fetch('/shared/notifications/test', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json',
                            },
                            credentials: 'include',
                            body: JSON.stringify('Info')
                        });
                        const text = await response.text();
                        try {
                            return JSON.parse(text);
                        } catch {
                            return { error: 'Not JSON', status: response.status, text: text.substring(0, 100) };
                        }
                    } catch (error) {
                        return { error: error.message };
                    }
                }
            """)
            
            if test_notification_response.get('success'):
                print("✅ Test notification sent successfully")
            else:
                print(f"⚠️ Test notification response: {test_notification_response}")
        except Exception as e:
            print(f"❌ Failed to send test notification: {e}")
        
        # Wait for potential notifications
        await asyncio.sleep(3)
        
        # Check for received notifications
        final_results = await page.evaluate("window.notificationResults")
        print(f"📊 Final Results:")
        print(f"  SignalR Connected: {final_results.get('connected', False)}")
        print(f"  Notifications Received: {len(final_results.get('notifications', []))}")
        print(f"  Errors: {len(final_results.get('errors', []))}")
        
        if final_results.get('notifications'):
            print("🎉 SUCCESS: Real-time notifications are working!")
            for i, notif in enumerate(final_results['notifications']):
                print(f"  Notification {i+1}: {notif}")
        else:
            print("⚠️ No real-time notifications were received")
        
        if final_results.get('errors'):
            print("❌ Errors encountered:")
            for error in final_results['errors']:
                print(f"  - {error}")
        
        # Take a screenshot for debugging
        await page.screenshot(path="testsprite_tests/notification_test_result.png")
        print("📸 Screenshot saved as notification_test_result.png")
        
        print("✅ Notification test completed")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()

    finally:
        print("🧹 Cleaning up...")
        if context:
            await context.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())
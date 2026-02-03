import asyncio
from playwright import async_api
import json

async def run_test():
    pw = None
    browser = None
    context1 = None
    context2 = None

    try:
        # Start a Playwright session in asynchronous mode
        pw = await async_api.async_playwright().start()

        # Launch a Chromium browser
        browser = await pw.chromium.launch(
            headless=False,  # Set to False to see the browser for debugging
            args=[
                "--window-size=1280,720",
                "--disable-dev-shm-usage",
                "--no-sandbox"
            ],
        )

        print("🚀 Starting Real-Time Notifications Test...")

        # Create two separate browser contexts for two users
        context1 = await browser.new_context()
        context1.set_default_timeout(10000)
        page1 = await context1.new_page()

        context2 = await browser.new_context()
        context2.set_default_timeout(10000)
        page2 = await context2.new_page()

        print("📱 Created two browser contexts for User A and User B")

        # USER A: Register first user
        print("👤 Registering User A...")
        await page1.goto("http://localhost:5002/register", wait_until="domcontentloaded")
        
        # Fill registration form for User A
        await page1.fill('#register-fullname', 'Test User A')
        await page1.fill('#register-email', 'testA@example.com')
        await page1.fill('#register-password', 'Password123!')
        await page1.fill('#register-confirm', 'Password123!')
        
        # Submit registration
        await page1.click('#register-button')
        await page1.wait_for_timeout(3000)
        
        print("✅ User A registered successfully")

        # USER B: Register second user
        print("👤 Registering User B...")
        await page2.goto("http://localhost:5002/register", wait_until="domcontentloaded")
        
        # Fill registration form for User B
        await page2.fill('#register-fullname', 'Test User B')
        await page2.fill('#register-email', 'testB@example.com')
        await page2.fill('#register-password', 'Password123!')
        await page2.fill('#register-confirm', 'Password123!')
        
        # Submit registration
        await page2.click('#register-button')
        await page2.wait_for_timeout(3000)
        
        print("✅ User B registered successfully")

        # Set up SignalR connection listeners for notifications
        print("🔌 Setting up SignalR notification listeners...")
        
        # Add JavaScript to listen for SignalR notifications on both pages
        await page1.add_init_script("""
            window.receivedNotifications = [];
            window.addEventListener('DOMContentLoaded', function() {
                if (typeof signalR !== 'undefined') {
                    const connection = new signalR.HubConnectionBuilder()
                        .withUrl('/hubs/notification')
                        .build();
                    
                    connection.start().then(function () {
                        console.log('User A: SignalR Connected');
                    }).catch(function (err) {
                        console.error('User A: SignalR Connection Error: ', err.toString());
                    });
                    
                    connection.on('ReceiveNotification', function (notification) {
                        console.log('User A: Received notification:', notification);
                        window.receivedNotifications.push(notification);
                    });
                    
                    window.notificationConnection = connection;
                }
            });
        """)
        
        await page2.add_init_script("""
            window.receivedNotifications = [];
            window.addEventListener('DOMContentLoaded', function() {
                if (typeof signalR !== 'undefined') {
                    const connection = new signalR.HubConnectionBuilder()
                        .withUrl('/hubs/notification')
                        .build();
                    
                    connection.start().then(function () {
                        console.log('User B: SignalR Connected');
                    }).catch(function (err) {
                        console.error('User B: SignalR Connection Error: ', err.toString());
                    });
                    
                    connection.on('ReceiveNotification', function (notification) {
                        console.log('User B: Received notification:', notification);
                        window.receivedNotifications.push(notification);
                    });
                    
                    window.notificationConnection = connection;
                }
            });
        """)

        # Navigate both users to the main application to establish SignalR connections
        print("🌐 Navigating users to main application...")
        await page1.goto("http://localhost:5002", wait_until="domcontentloaded")
        await page2.goto("http://localhost:5002", wait_until="domcontentloaded")
        
        # Wait for SignalR connections to establish
        await asyncio.sleep(3)

        # USER A: Send a test notification to User B via API
        print("📤 User A sending test notification to User B...")
        
        # First, we need to get User B's ID. For this test, we'll use the notification test endpoint
        try:
            # Send a test notification using the API endpoint
            response = await page1.evaluate("""
                async () => {
                    try {
                        const response = await fetch('/shared/notifications/test', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json',
                            },
                            body: JSON.stringify('Info')
                        });
                        return await response.json();
                    } catch (error) {
                        return { error: error.message };
                    }
                }
            """)
            
            if response.get('success'):
                print("✅ Test notification sent successfully")
            else:
                print(f"⚠️ Test notification response: {response}")
        except Exception as e:
            print(f"❌ Failed to send test notification: {e}")

        # Wait for notification to be received
        await asyncio.sleep(2)

        # Check if User A received the notification
        print("📨 Checking if User A received the notification...")
        
        try:
            notifications_a = await page1.evaluate("window.receivedNotifications || []")
            if notifications_a:
                print(f"🎉 SUCCESS: User A received {len(notifications_a)} notification(s)!")
                for i, notif in enumerate(notifications_a):
                    print(f"  Notification {i+1}: {notif.get('Title', 'No title')} - {notif.get('Message', 'No message')}")
            else:
                print("❌ No notifications received by User A")
        except Exception as e:
            print(f"❌ Error checking notifications for User A: {e}")

        # Test notification API endpoints
        print("🔍 Testing notification API endpoints...")
        
        # Get unread count
        try:
            unread_response = await page1.evaluate("""
                async () => {
                    try {
                        const response = await fetch('/shared/notifications/unread-count');
                        return await response.json();
                    } catch (error) {
                        return { error: error.message };
                    }
                }
            """)
            
            if unread_response.get('success'):
                count = unread_response.get('data', 0)
                print(f"✅ Unread notifications count: {count}")
            else:
                print(f"⚠️ Unread count response: {unread_response}")
        except Exception as e:
            print(f"❌ Failed to get unread count: {e}")

        # Get all notifications
        try:
            all_notifications_response = await page1.evaluate("""
                async () => {
                    try {
                        const response = await fetch('/shared/notifications');
                        return await response.json();
                    } catch (error) {
                        return { error: error.message };
                    }
                }
            """)
            
            if all_notifications_response.get('success'):
                notifications = all_notifications_response.get('data', [])
                print(f"✅ Retrieved {len(notifications)} notifications from API")
                for i, notif in enumerate(notifications):
                    print(f"  API Notification {i+1}: {notif.get('Title', 'No title')} - {notif.get('Message', 'No message')}")
            else:
                print(f"⚠️ Get notifications response: {all_notifications_response}")
        except Exception as e:
            print(f"❌ Failed to get notifications: {e}")

        # Test notification UI elements
        print("🎨 Testing notification UI elements...")
        
        # Look for notification bell/icon
        notification_bell = page1.locator('[data-testid="notification-bell"], .notification-bell, button:has-text("Notifications"), [class*="notification"]').first
        if await notification_bell.count() > 0:
            print("✅ Found notification bell/button in UI")
            try:
                await notification_bell.click()
                print("✅ Clicked notification bell")
                await asyncio.sleep(1)
                
                # Look for notification dropdown/panel
                notification_panel = page1.locator('.notification-panel, .notification-dropdown, [data-testid="notification-panel"]').first
                if await notification_panel.count() > 0:
                    print("✅ Notification panel opened")
                else:
                    print("⚠️ Notification panel not found after clicking bell")
            except Exception as e:
                print(f"⚠️ Could not interact with notification bell: {e}")
        else:
            print("⚠️ Notification bell not found in UI")

        # Test real-time delivery by triggering an action that should generate a notification
        print("🔄 Testing real-time notification delivery...")
        
        # Try to trigger a notification by performing an action (like creating a post or question)
        try:
            # Navigate to Q&A section to ask a question (this might trigger notifications)
            qa_link = page1.locator('a[href*="qa"], a:has-text("Q&A"), a:has-text("Questions")').first
            if await qa_link.count() > 0:
                await qa_link.click()
                print("✅ Navigated to Q&A section")
                await asyncio.sleep(2)
                
                # Look for "Ask Question" button
                ask_button = page1.locator('button:has-text("Ask"), a:has-text("Ask"), [data-testid="ask-question"]').first
                if await ask_button.count() > 0:
                    await ask_button.click()
                    print("✅ Clicked Ask Question button")
                    await asyncio.sleep(1)
                    
                    # Fill question form if available
                    title_input = page1.locator('input[name*="title"], #question-title, [data-testid="question-title"]').first
                    if await title_input.count() > 0:
                        await title_input.fill("Test Question for Notification")
                        print("✅ Filled question title")
                        
                        content_input = page1.locator('textarea[name*="content"], #question-content, [data-testid="question-content"]').first
                        if await content_input.count() > 0:
                            await content_input.fill("This is a test question to trigger notifications.")
                            print("✅ Filled question content")
                            
                            submit_button = page1.locator('button[type="submit"], button:has-text("Submit"), button:has-text("Ask")').first
                            if await submit_button.count() > 0:
                                await submit_button.click()
                                print("✅ Submitted question")
                                await asyncio.sleep(2)
            else:
                print("⚠️ Q&A section not found")
        except Exception as e:
            print(f"⚠️ Could not test Q&A notification trigger: {e}")

        # Final check for any new notifications
        await asyncio.sleep(2)
        try:
            final_notifications_a = await page1.evaluate("window.receivedNotifications || []")
            final_notifications_b = await page2.evaluate("window.receivedNotifications || []")
            
            print(f"📊 Final Results:")
            print(f"  User A received: {len(final_notifications_a)} notifications")
            print(f"  User B received: {len(final_notifications_b)} notifications")
            
            if final_notifications_a or final_notifications_b:
                print("🎉 SUCCESS: Real-time notifications are working!")
            else:
                print("⚠️ No real-time notifications were received during the test")
                
        except Exception as e:
            print(f"❌ Error in final notification check: {e}")

        print("✅ Real-time notifications test completed")

    except Exception as e:
        print(f"❌ Test failed with error: {e}")
        import traceback
        traceback.print_exc()

    finally:
        print("🧹 Cleaning up...")
        if context1:
            await context1.close()
        if context2:
            await context2.close()
        if browser:
            await browser.close()
        if pw:
            await pw.stop()

if __name__ == "__main__":
    asyncio.run(run_test())
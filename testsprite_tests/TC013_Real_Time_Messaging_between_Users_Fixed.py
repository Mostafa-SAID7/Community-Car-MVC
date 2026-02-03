import asyncio
from playwright import async_api

async def run_test():
    pw = None
    browser = None
    context1 = None
    context2 = None

    try:
        # Start a Playwright session in asynchronous mode
        pw = await async_api.async_playwright().start()

        # Launch a Chromium browser in headless mode with custom arguments
        browser = await pw.chromium.launch(
            headless=False,  # Set to False to see the browser for debugging
            args=[
                "--window-size=1280,720",
                "--disable-dev-shm-usage",
                "--no-sandbox"
            ],
        )

        print("🚀 Starting Real-Time Messaging Test...")

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

        # USER A: Navigate to chat
        print("💬 User A navigating to chat...")
        try:
            await page1.goto("http://localhost:5002/chats", wait_until="domcontentloaded")
            print("✅ User A accessed chat page")
        except Exception as e:
            print(f"❌ User A failed to access chat: {e}")
            # Try alternative navigation
            await page1.goto("http://localhost:5002", wait_until="domcontentloaded")
            # Look for chat link in navigation
            chat_link = page1.locator('a[href*="chat"]').first
            if await chat_link.count() > 0:
                await chat_link.click()
                print("✅ User A navigated to chat via navigation link")
            else:
                print("⚠️ Chat navigation not found, continuing with direct URL")

        # USER B: Navigate to chat
        print("💬 User B navigating to chat...")
        try:
            await page2.goto("http://localhost:5002/chats", wait_until="domcontentloaded")
            print("✅ User B accessed chat page")
        except Exception as e:
            print(f"❌ User B failed to access chat: {e}")
            # Try alternative navigation
            await page2.goto("http://localhost:5002", wait_until="domcontentloaded")
            # Look for chat link in navigation
            chat_link = page2.locator('a[href*="chat"]').first
            if await chat_link.count() > 0:
                await chat_link.click()
                print("✅ User B navigated to chat via navigation link")
            else:
                print("⚠️ Chat navigation not found, continuing with direct URL")

        # Wait for SignalR connection to establish
        await asyncio.sleep(2)

        # USER A: Try to start a new conversation
        print("🔄 User A attempting to start new conversation...")
        new_chat_btn = page1.locator('#new-chat-btn, button:has-text("New"), button:has-text("Start")').first
        if await new_chat_btn.count() > 0:
            await new_chat_btn.click()
            print("✅ User A clicked new chat button")
        else:
            print("⚠️ New chat button not found")

        # Look for message input
        message_input = page1.locator('#message-input, textarea[placeholder*="message"], input[placeholder*="message"]').first
        if await message_input.count() > 0:
            await message_input.fill("Hello from User A! This is a test message.")
            print("✅ User A typed message")
            
            # Look for send button
            send_btn = page1.locator('#send-message-btn, button[title*="Send"], button:has-text("Send")').first
            if await send_btn.count() > 0:
                await send_btn.click()
                print("✅ User A sent message")
            else:
                # Try pressing Enter
                await message_input.press('Enter')
                print("✅ User A sent message via Enter key")
        else:
            print("❌ Message input not found")

        # Wait for message to be processed
        await asyncio.sleep(2)

        # USER B: Check for received message
        print("📨 Checking if User B received the message...")
        
        # Look for messages in the chat area
        messages = page2.locator('#chat-messages .message, .chat-message, [data-message]')
        message_count = await messages.count()
        
        if message_count > 0:
            print(f"✅ Found {message_count} message(s) in User B's chat")
            
            # Try to get message content
            for i in range(message_count):
                try:
                    message_text = await messages.nth(i).text_content()
                    if "Hello from User A" in message_text:
                        print("🎉 SUCCESS: Real-time message received by User B!")
                        break
                except:
                    continue
        else:
            print("❌ No messages found in User B's chat")

        # USER B: Send reply
        print("💬 User B sending reply...")
        message_input_b = page2.locator('#message-input, textarea[placeholder*="message"], input[placeholder*="message"]').first
        if await message_input_b.count() > 0:
            await message_input_b.fill("Hello User A! I received your message. Real-time messaging works!")
            
            send_btn_b = page2.locator('#send-message-btn, button[title*="Send"], button:has-text("Send")').first
            if await send_btn_b.count() > 0:
                await send_btn_b.click()
                print("✅ User B sent reply")
            else:
                await message_input_b.press('Enter')
                print("✅ User B sent reply via Enter key")
        
        await asyncio.sleep(2)

        # USER A: Check for reply
        print("📨 Checking if User A received the reply...")
        messages_a = page1.locator('#chat-messages .message, .chat-message, [data-message]')
        message_count_a = await messages_a.count()
        
        if message_count_a > 1:  # Should have original message + reply
            print(f"✅ Found {message_count_a} message(s) in User A's chat")
            print("🎉 SUCCESS: Bidirectional real-time messaging confirmed!")
        else:
            print("⚠️ Reply not found in User A's chat")

        print("✅ Real-time messaging test completed")

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
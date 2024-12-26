document.addEventListener('DOMContentLoaded', function() {
    const currentUserIdElement = document.getElementById("currentUserId");
    const currentUserNameElement = document.getElementById("currentUserName");
    const chatContainerElement = document.querySelector(".chat-container");
    const messagesListElement = document.querySelector(".chat-messages");
    const messageInputElement = document.querySelector("#messageInput");

    if (!currentUserIdElement || !currentUserNameElement) {
        console.log("Chat için gerekli elementler bulunamadı");
        return;
    }

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Information)
        .build();

    let currentUserId = currentUserIdElement.value;
    let currentUserName = currentUserNameElement.value;
    let chatContainer = chatContainerElement;
    let messagesList = messagesListElement;
    let messageInput = messageInputElement;
    let receiverId = null;
    let isConnected = false;

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected!");
            isConnected = true;
            await connection.invoke("JoinChat", currentUserId, currentUserName);
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    }

    connection.onclose(async () => {
        isConnected = false;
        await start();
    });

    start();

    connection.on("ReceivePrivateMessage", function (senderId, message, timestamp) {
        addMessageToChat(senderId, message, timestamp);
    });

    connection.on("LoadMessages", function (messages) {
        if (messagesList) {
            messagesList.innerHTML = '';
            messages.forEach(msg => {
                addMessageToChat(msg.senderId, msg.content, msg.timestamp);
            });
            messagesList.scrollTop = messagesList.scrollHeight;
        }
    });

    window.sendMessage = async function() {
        if (!isConnected) {
            console.log("Bağlantı bekleniyor...");
            return;
        }

        if (messageInput && messageInput.value && receiverId) {
            const message = messageInput.value.trim();
            if (message) {
                try {
                    await connection.invoke("SendPrivateMessage", currentUserId, receiverId, message);
                    addMessageToChat(currentUserId, message, new Date());
                    messageInput.value = '';
                } catch (err) {
                    console.error(err);
                    messageInput.value = message;
                }
            }
        }
    }

    window.startChat = async function(userId, userName) {
        if (!isConnected) {
            console.log("Bağlantı bekleniyor...");
            return;
        }

        receiverId = userId;
        if (chatContainer) {
            const headerElement = chatContainer.querySelector(".chat-header h3");
            if (headerElement) {
                headerElement.textContent = `Chat with ${userName}`;
            }
            chatContainer.classList.remove("chat-hidden");
            await connection.invoke("LoadMessages", currentUserId, receiverId);
        }
    }

    function addMessageToChat(senderId, message, timestamp) {
        if (!messagesList) return;

        const messageDiv = document.createElement("div");
        messageDiv.classList.add("message");
        messageDiv.classList.add(senderId === currentUserId ? "message-sent" : "message-received");
        
        const time = new Date(timestamp).toLocaleTimeString();
        messageDiv.innerHTML = `
            <div class="message-content">${message}</div>
            <div class="message-time">${time}</div>
        `;
        
        messagesList.appendChild(messageDiv);
        messagesList.scrollTop = messagesList.scrollHeight;
    }

    if (messageInput) {
        messageInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                sendMessage();
            }
        });
    }
});
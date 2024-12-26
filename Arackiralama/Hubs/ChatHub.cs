using Microsoft.AspNetCore.SignalR;

using System.Security.Claims;
using AracKiralama.Models;
using Microsoft.AspNetCore.Identity;

namespace AracKiralama.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(MessageRepository messageRepository, UserManager<AppUser> userManager)
        {
            _messageRepository = messageRepository;
            _userManager = userManager;
        }

        public async Task SendPrivateMessage(string fromUserId, string toUserId, string message)
        {
            var timestamp = DateTime.Now;
            var newMessage = new Message
            {
                SenderId = fromUserId,
                ReceiverId = toUserId,
                Content = message,
                Timestamp = timestamp,
                IsRead = false
            };

            await _messageRepository.AddAsync(newMessage);

            // Sadece alıcının grubuna mesajı gönder
            await Clients.Group(toUserId).SendAsync("ReceivePrivateMessage", fromUserId, message, timestamp);
        }

        public async Task LoadMessages(string userId1, string userId2)
        {
            var messages = await _messageRepository.GetConversation(userId1, userId2);
            await Clients.Caller.SendAsync("LoadMessages", messages);
        }

        public async Task JoinChat(string userId, string userName)
        {
            // Kullanıcıyı kendi ID'si ile bir gruba ekle
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Bağlantı koptuğunda gruptan çıkar
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
} 
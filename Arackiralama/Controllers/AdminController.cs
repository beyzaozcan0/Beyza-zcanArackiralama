using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using AracKiralama.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AracKiralama.Models;
using Microsoft.AspNetCore.Authorization;


namespace AracKiralama.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
       
        private readonly INotyfService _notyfService;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly MessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;
        public AdminController(INotyfService notyfService, IMapper mapper, UserRepository userRepository, MessageRepository messageRepository, UserManager<AppUser> userManager)
        {
            _notyfService = notyfService;
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _userManager = userManager;
           
        }
        public IActionResult Index()
        {
            return View();
        }    

        public async Task<IActionResult> Messages()
        {
            var admin = await _userManager.GetUserAsync(User);
            ViewBag.AdminId = admin.Id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserConversations()
        {
            var admin = await _userManager.GetUserAsync(User);
            var messages = await _messageRepository.GetAllAsync();
            
            var conversations = messages
                .Where(m => m.SenderId == admin.Id || m.ReceiverId == admin.Id)
                .GroupBy(m => m.SenderId == admin.Id ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    UserId = g.Key,
                    UserName = _userManager.FindByIdAsync(g.Key).Result.UserName,
                    LastMessage = g.OrderByDescending(m => m.Timestamp).First().Content,
                    UnreadCount = g.Count(m => !m.IsRead && m.ReceiverId == admin.Id),
                    LastMessageTime = g.Max(m => m.Timestamp)
                })
                .OrderByDescending(c => c.LastMessageTime);

            return Json(conversations);
        }
    }
}

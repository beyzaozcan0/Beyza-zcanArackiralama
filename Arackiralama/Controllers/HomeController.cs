using AracKiralama.Models;
using AracKiralama.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace AracKiralama.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notfy;
        private readonly CarRepository _carRepository;
        private readonly RentalRepository _rentalRepository;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            INotyfService notfy,
            CarRepository carRepository,
            RentalRepository rentalRepository,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _notfy = notfy;
            _carRepository = carRepository;
            _rentalRepository = rentalRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllWithDetailsAsync();
            return View(cars);
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _notfy.Error("Lütfen önce giriş yapın.");
                return RedirectToAction("Login", "User");
            }

            var rentals = await _rentalRepository.GetUserRentalsAsync(user.Id);
            
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.UserId = user.Id;

            var adminUser = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminUser.Any())
            {
                ViewBag.AdminId = adminUser.First().Id;
                ViewBag.AdminName = adminUser.First().UserName;
            }

            return View(rentals);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> RentCar([FromBody] RentalRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Lütfen önce giriş yapın." });
                }

                var user = await _userManager.GetUserAsync(User);
                var car = await _carRepository.GetByIdAsync(request.CarId);

                if (car == null)
                {
                    return Json(new { success = false, message = "Araç bulunamadı." });
                }

                if (!car.IsAvailable)
                {
                    return Json(new { success = false, message = "Bu araç şu anda müsait değil." });
                }

                var totalDays = (request.EndDate - request.StartDate).Days + 1;
                var totalPrice = car.DailyPrice * totalDays;

                var rental = new Rental
                {
                    CarId = car.Id,
                    UserId = user.Id,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalPrice = totalPrice,
                    Status = RentalStatus.Pending,
                    IsCompleted = false
                };

                await _rentalRepository.AddAsync(rental);
                _notfy.Success("Kiralama talebiniz alındı. Yönetici onayından sonra bilgilendirileceksiniz.");
                
                return Json(new { 
                    success = true, 
                    message = "Kiralama talebiniz alındı. Yönetici onayından sonra bilgilendirileceksiniz." 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        public class RentalRequest
        {
            public int CarId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}

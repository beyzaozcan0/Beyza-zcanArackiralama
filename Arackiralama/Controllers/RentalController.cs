using AracKiralama.Models;
using AracKiralama.Repositories;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace AracKiralama.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RentalController : Controller
    {
        private readonly RentalRepository _rentalRepository;
        private readonly CarRepository _carRepository;
        private readonly INotyfService _notyf;

        public RentalController(
            RentalRepository rentalRepository,
            CarRepository carRepository,
            INotyfService notyf)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rentals = await _rentalRepository.GetAllWithDetailsAsync();
            return Json(rentals);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, RentalStatus status)
        {
            try
            {
                var rental = await _rentalRepository.GetByIdAsync(id);
                if (rental == null)
                    return Json(new { success = false, message = "Kiralama bulunamadı." });

                rental.Status = status;
                
                if (status == RentalStatus.Approved)
                {
                    var car = await _carRepository.GetByIdAsync(rental.CarId);
                    if (car != null)
                    {
                        car.IsAvailable = false;
                        await _carRepository.UpdateAsync(car);
                    }
                }

                await _rentalRepository.UpdateAsync(rental);

                string message = status == RentalStatus.Approved ? 
                    "Kiralama talebi onaylandı." : 
                    "Kiralama talebi reddedildi.";

                return Json(new { success = true, message = message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRental(int id)
        {
            try
            {
                var rental = await _rentalRepository.GetByIdAsync(id);
                if (rental == null)
                    return Json(new { success = false, message = "Kiralama bulunamadı." });

                // Kiralama durumunu güncelle
                rental.IsCompleted = true;
                rental.Status = RentalStatus.Completed;
                await _rentalRepository.UpdateAsync(rental);

                // Arabayı tekrar müsait yap
                var car = await _carRepository.GetByIdAsync(rental.CarId);
                if (car != null)
                {
                    car.IsAvailable = true;
                    await _carRepository.UpdateAsync(car);
                }

                return Json(new { success = true, message = "Araç başarıyla teslim alındı." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
            }
        }
    }
} 
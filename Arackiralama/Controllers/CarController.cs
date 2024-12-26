using AracKiralama.Models;
using AracKiralama.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace AracKiralama.Controllers
{
    public class CarController : Controller
    {
        private readonly CarRepository _carRepository;
        private readonly BrandRepository _brandRepository;
        private readonly IWebHostEnvironment _environment;

        public CarController(CarRepository carRepository, BrandRepository brandRepository, IWebHostEnvironment environment)
        {
            _carRepository = carRepository;
            _brandRepository = brandRepository;
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cars = await _carRepository.GetAllWithDetailsAsync();
                return Json(cars);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _brandRepository.GetAllAsync();
            return Json(brands);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] Car car, IFormFile? imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "car-images");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    car.ImagePath = "/car-images/" + uniqueFileName;
                }

                await _carRepository.AddAsync(car);
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false });
            }

          
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _carRepository.DeleteAsync(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromForm] Car car, IFormFile? imageFile)
        {
            try
            {
                
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Eski resmi sil
                    if (!string.IsNullOrEmpty(car.ImagePath))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, car.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    // Yeni resmi kaydet
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "car-images");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    car.ImagePath = "/car-images/" + uniqueFileName;
                }
                car.ImagePath = car.ImagePath;
                await _carRepository.UpdateAsync(car);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
} 
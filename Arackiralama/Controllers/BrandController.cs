using AracKiralama.Models;
using AracKiralama.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AracKiralama.Controllers
{
    public class BrandController : Controller
    {
        private readonly BrandRepository _brandRepository;

        public BrandController(BrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandRepository.GetAllAsync();
            return Json(brands);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Brand brand)
        {
            try
            {
                await _brandRepository.AddAsync(brand);
                return Json(new { success = true });
            }
            catch (Exception ex) {
                return Json(new { success = false });
            }
                
           
        
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _brandRepository.DeleteAsync(id);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Brand brand)
        {
            try
            {
                await _brandRepository.UpdateAsync(brand);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
    }
} 
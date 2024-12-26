using AracKiralama.Models;
using Microsoft.EntityFrameworkCore;

namespace AracKiralama.Repositories
{
    public class CarRepository : GenericRepository<Car>
    {
        public CarRepository(AppDbContext context) : base(context, context.Cars)
        {
        }

        public async Task<IEnumerable<object>> GetAllWithDetailsAsync()
        {
            return await _context.Cars
                .Include(c => c.Brand)
                .Select(c => new
                {
                    c.Id,
                    c.BrandId,
                    BrandName = c.Brand.Name,
                    c.Model,
                    c.Year,
                    c.Plate,
                    c.DailyPrice,
                    c.IsAvailable,
                    c.ImagePath
                })
                .ToListAsync();
        }
    }
} 
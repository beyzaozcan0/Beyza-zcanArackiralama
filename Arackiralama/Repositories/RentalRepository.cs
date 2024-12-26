using AracKiralama.Models;
using Microsoft.EntityFrameworkCore;

namespace AracKiralama.Repositories
{
    public class RentalRepository : GenericRepository<Rental>
    {
        public RentalRepository(AppDbContext context) : base(context, context.Rentals)
        {
        }

        public async Task<IEnumerable<object>> GetAllWithDetailsAsync()
        {
            return await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.Brand)
                .Include(r => r.User)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new
                {
                    r.Id,
                    CarInfo = $"{r.Car.Brand.Name} {r.Car.Model} - {r.Car.Plate}",
                    r.UserId,
                    UserName = r.User.UserName,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice,
                    r.Status,
                    r.IsCompleted,
                    r.Car.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetUserRentalsAsync(string userId)
        {
            return await _context.Rentals
                .Include(r => r.Car)
                .ThenInclude(c => c.Brand)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new
                {
                    r.Id,
                    CarInfo = $"{r.Car.Brand.Name} {r.Car.Model}",
                    r.Car.ImagePath,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice,
                    r.IsCompleted,
                    r.Status
                })
                .ToListAsync();
        }
    }
} 
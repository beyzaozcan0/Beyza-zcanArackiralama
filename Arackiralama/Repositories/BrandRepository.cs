using AracKiralama.Models;

namespace AracKiralama.Repositories
{
    public class BrandRepository : GenericRepository<Brand>
    {
        public BrandRepository(AppDbContext context) : base(context, context.Brands)
        {
        }
    }
} 
using Microsoft.AspNetCore.Identity;

namespace AracKiralama.Models
{
    public class AppUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
      
        public ICollection<Rental> Rentals { get; set; } // Kullanıcının kiralamaları
    }
}

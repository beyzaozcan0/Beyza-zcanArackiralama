using System.ComponentModel.DataAnnotations.Schema;

namespace AracKiralama.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]  
        public decimal TotalPrice { get; set; }
        public bool IsCompleted { get; set; }
        public RentalStatus Status { get; set; } = RentalStatus.Pending;
    }

    public enum RentalStatus
    {
        Pending,    // Onay Bekliyor
        Approved,   // Onaylandı
        Rejected,   // Reddedildi
        Completed   // Tamamlandı
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;

namespace AracKiralama.Models
{
    public class Car
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string Plate { get; set; }
         [Column(TypeName = "decimal(18,2)")]
        public decimal DailyPrice { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? ImagePath { get; set; }
        public Brand Brand { get; set; }
    }
}

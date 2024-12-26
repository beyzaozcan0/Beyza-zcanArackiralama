namespace AracKiralama.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; } // Marka Adı
        public ICollection<Car> Cars { get; set; } // Bu markaya ait arabalar
    }
}

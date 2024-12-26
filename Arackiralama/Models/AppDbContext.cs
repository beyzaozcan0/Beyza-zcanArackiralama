using AracKiralama.Models;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt.Extensions;

namespace AracKiralama.Models
{
    public class AppDbContext : IdentityDbContext <AppUser,AppRole,string>
    {
        private readonly IConfiguration _config;
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options)
        {
            _config = config;
        }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<Rental> Rentals { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminrolu = Guid.NewGuid().ToString(); // admin hesabı için rol oluştur
            var adminhesabi = Guid.NewGuid().ToString();
            modelBuilder.Entity<Message>()
           .HasOne(m => m.Sender)
           .WithMany()
           .HasForeignKey(m => m.SenderId)
           .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                });
            modelBuilder.Entity<Message>()
               .HasOne(m => m.Receiver)
               .WithMany()
               .HasForeignKey(m => m.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict); modelBuilder.Entity<AppRole>().HasData(
                new AppRole
                {
                    Id = adminrolu,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });modelBuilder.Entity<AppUser>().HasData(new AppUser
                {
                    Id = adminhesabi,
                    UserName = "admin",
                    FirstName= "admin",
                    LastName= "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "beyza@beyza.com",
                    NormalizedEmail = "BEYZA@BEYZA.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<AppUser>().HashPassword(null, "Admin.!")
                });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminhesabi,
                    RoleId = adminrolu
                });
            base.OnModelCreating(modelBuilder);
        }
    }
}

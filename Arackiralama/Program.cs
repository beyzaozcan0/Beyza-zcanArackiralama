using AracKiralama.Repositories;
using AspNetCoreHero.ToastNotification;
using AracKiralama.Models;
using AracKiralama.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using AracKiralama.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(GenericRepository<>));
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<CarRepository>();
builder.Services.AddScoped<RentalRepository>();
builder.Services.AddScoped<MessageRepository>();


builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("baglan"));
});


builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.Password.RequireDigit = true; //Parolanın en az bir rakam (digit) içermesi gerektiğini belirtir.
    opt.Password.RequireLowercase = true;  // Parolanın en az bir küçük harf içermesi gerektiğini belirtir
    opt.Password.RequireUppercase = true; // Parolanın en az bir büyük harf içermesi gerektiğini belirtir. 
    opt.Password.RequireNonAlphanumeric = true;//Parolanın en az bir alfasayısal olmayan karakter içermesi gerektiğini belirtir.
    opt.Password.RequiredLength = 6; // Parolanın minimum uzunluğunu belirler. 
    opt.User.RequireUniqueEmail = true; //Kullanıcıların benzersiz bir e-posta adresine sahip olmalarını sağlar.
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "CookieAuthApp";
        opt.ExpireTimeSpan = TimeSpan.FromDays(3);
        opt.LoginPath = "/User/Login";
        opt.LogoutPath = "/User/Logout";
        opt.AccessDeniedPath = "/User/Login";
        opt.SlidingExpiration = false;
    });


builder.Services.AddSignalR();

// Session desteği ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Add this line
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.UseSession();

app.Run();

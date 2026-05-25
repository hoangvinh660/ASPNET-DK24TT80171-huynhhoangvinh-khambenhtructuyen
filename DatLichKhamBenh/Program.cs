using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DatLichKhamBenh.Models;
// using DatLichKhamBenh.Services;

var builder = WebApplication.CreateBuilder(args);

// Dang ky MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Dang ky DbContext (EF Core + SQL Server)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DatLichKhamBenh")));

// TODO Buoc 8: Dang ky EmailService (MailKit gui mail qua Gmail SMTP)
// builder.Services.AddScoped<IEmailService, EmailService>();
// builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Bat Session (luu thong tin tam thoi nhu gio dat lich da chon)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromHours(2);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

// Cookie Authentication (dung cho dang nhap tu code o Buoc 4)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/TaiKhoan/DangNhap";
        opt.LogoutPath = "/TaiKhoan/DangXuat";
        opt.AccessDeniedPath = "/TaiKhoan/TuChoi";
        opt.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

// Tu dong chay migration + nap du lieu mau (chi nap neu DB rong)
SeedData.Initialize(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Route mac dinh.
// Khu vuc /Admin/* duoc khai bao bang [Route("Admin/[controller]/[action]")]
// truc tiep tren tung admin controller (xem Controllers/Admin/* o Buoc 7).
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

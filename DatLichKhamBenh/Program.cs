using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Services;

var builder = WebApplication.CreateBuilder(args);

// Dang ky MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// Dang ky DbContext (EF Core + SQL Server)
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DatLichKhamBenh")));

// Dang ky EmailService (MailKit gui mail qua Gmail SMTP).
// Cau hinh DOC TU DB (bang CauHinhEmail) qua IEmailSettingsProvider.
// Lan dau khi DB chua co record nao, provider fallback ve appsettings.json
// -> "EmailSettings" (gia tri seed san khi SeedData.Initialize chay).
// Admin co the chinh sua/tat o /Admin/Email.
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSettingsProvider, EmailSettingsProvider>();
builder.Services.AddScoped<IEmailService, EmailService>();

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
    app.UseExceptionHandler("/TrangChu/Error");
    app.UseHsts();
    // Chi bat HTTPS redirect trong moi truong Production (de tranh canh bao
    // "Failed to determine the https port for redirect" khi chay HTTP local).
    app.UseHttpsRedirection();
}

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
    pattern: "{controller=TrangChu}/{action=Index}/{id?}");

app.Run();

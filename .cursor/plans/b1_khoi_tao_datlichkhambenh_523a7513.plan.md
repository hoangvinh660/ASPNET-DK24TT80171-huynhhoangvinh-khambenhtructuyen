---
name: B1 Khoi Tao DatLichKhamBenh
overview: "Bước 1 của lộ trình: tạo project ASP.NET Core MVC .NET 8 mang tên `DatLichKhamBenh`, cài 4 gói NuGet cần thiết, cấu hình `appsettings.json` (ConnectionString + EmailSettings) và dựng sẵn cấu trúc thư mục cho các bước sau."
todos:
  - id: create-project
    content: Tạo solution + project ASP.NET Core MVC .NET 8 tên DatLichKhamBenh
    status: completed
  - id: install-nuget
    content: "Cài 4 NuGet: EF Core SqlServer/Tools/Design + MailKit; đảm bảo dotnet-ef CLI"
    status: completed
  - id: config-appsettings
    content: "Cập nhật appsettings.json: ConnectionString + EmailSettings"
    status: completed
  - id: config-program
    content: "Cập nhật Program.cs: Session, Cookie Auth, route mặc định, chỗ chờ cho DbContext/EmailService"
    status: completed
  - id: create-folders
    content: "Tạo khung thư mục rỗng: Models, Models/ViewModels, Services, Controllers/Admin, Views/Admin"
    status: completed
  - id: smoke-test
    content: dotnet build + dotnet run để kiểm tra build pass và trang Home mở được
    status: completed
isProject: false
---

## Phạm vi Bước 1 (dừng để bạn kiểm tra trước khi sang Bước 2)

Chỉ làm phần **khởi tạo project + cấu hình**. Chưa tạo Models, chưa tạo Controller chức năng, chưa Migration.

---

## 1. Tạo Solution và Project

Chạy tại workspace root `c:\Users\Admin\Desktop\dangkykhambenh`:

```powershell
dotnet new sln -n DatLichKhamBenh
dotnet new mvc -n DatLichKhamBenh -f net8.0
dotnet sln add DatLichKhamBenh/DatLichKhamBenh.csproj
```

Kết quả: thư mục `DatLichKhamBenh/` với template MVC chuẩn (đã sẵn Bootstrap 5, jQuery, wwwroot, Views).

---

## 2. Cài NuGet packages

Chạy trong `DatLichKhamBenh/`:

```powershell
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Tools     --version 8.0.10
dotnet add package Microsoft.EntityFrameworkCore.Design    --version 8.0.10
dotnet add package MailKit                                 --version 4.8.0
```

(Bootstrap 5 + Chart.js sẽ thêm trực tiếp vào `wwwroot/lib` ở Bước 9. Không cần BCrypt vì đã chọn lưu mật khẩu plain text.)

Cài CLI `dotnet-ef` nếu chưa có (để Bước 2 chạy migration):

```powershell
dotnet tool install --global dotnet-ef --version 8.0.10
```

---

## 3. Cấu hình `appsettings.json`

Sửa file `DatLichKhamBenh/appsettings.json` thành:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DatLichKhamBenh": "Server=DESKTOP-IL8JOMQ\\BARTENDER;Database=DatLichKhamBenh;User Id=sa;Password=Admin@123;TrustServerCertificate=True"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "Phòng khám DatLichKhamBenh",
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "app-password-16-ky-tu"
  }
}
```

`SenderEmail` / `SenderPassword` để placeholder — bạn thay sau khi tạo App Password Gmail (hướng dẫn ở Bước 8).

---

## 4. Cập nhật `Program.cs`

Sửa `DatLichKhamBenh/Program.cs` để:
- Đăng ký `AppDbContext` (class sẽ tạo ở Bước 2, tạm thời comment hoặc dùng `// TODO`).
- Bật **Session** + **Cookie Authentication** (dùng cho đăng nhập tự code ở Bước 4).
- Đăng ký `IEmailService` (interface sẽ tạo ở Bước 8, tạm thời để chỗ).

Nội dung chính:

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.EntityFrameworkCore;
// using DatLichKhamBenh.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// TODO Bước 2: builder.Services.AddDbContext<AppDbContext>(opt =>
//     opt.UseSqlServer(builder.Configuration.GetConnectionString("DatLichKhamBenh")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromHours(2);
    opt.Cookie.HttpOnly = true;
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath  = "/TaiKhoan/DangNhap";
        opt.AccessDeniedPath = "/TaiKhoan/TuChoi";
        opt.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TrangChu}/{action=Index}/{id?}");

app.Run();
```

Lưu ý: route mặc định trỏ về `TrangChuController` (sẽ tạo ở Bước 5). Bước 1 để tạm template `HomeController` đang có, sẽ đổi tên ở bước sau.

---

## 5. Tạo sẵn cấu trúc thư mục

Tạo các thư mục rỗng (chỉ để khung sẵn, file thực tế thêm ở các bước sau). Có thể dùng `.gitkeep`:

- `DatLichKhamBenh/Models/`
- `DatLichKhamBenh/Models/ViewModels/`
- `DatLichKhamBenh/Services/`
- `DatLichKhamBenh/Controllers/Admin/`
- `DatLichKhamBenh/Views/Admin/`

(Các thư mục `Controllers/`, `Views/`, `wwwroot/` đã có sẵn từ template.)

---

## 6. Smoke test

```powershell
cd DatLichKhamBenh
dotnet build
dotnet run
```

Mục tiêu: build pass, mở `https://localhost:xxxx` thấy trang Home template mặc định. Chưa kết nối DB nên không lỗi DB.

---

## Cây thư mục sau Bước 1

```
dangkykhambenh/
├── DatLichKhamBenh.sln
└── DatLichKhamBenh/
    ├── Controllers/
    │   ├── Admin/                  (rỗng, .gitkeep)
    │   └── HomeController.cs       (sẽ đổi tên ở Bước 5)
    ├── Models/                     (rỗng)
    │   └── ViewModels/             (rỗng)
    ├── Services/                   (rỗng)
    ├── Views/                      (template mặc định)
    │   └── Admin/                  (rỗng)
    ├── wwwroot/                    (template mặc định)
    ├── Program.cs                  (đã cập nhật)
    ├── appsettings.json            (đã cập nhật)
    └── DatLichKhamBenh.csproj
```

---

## Lộ trình tổng thể (để bạn nắm context, KHÔNG làm trong Bước 1)

- **Bước 2** — Tạo 6 Entity + `AppDbContext` + Migration `InitialCreate` + `dotnet ef database update`.
- **Bước 3** — Lớp `SeedData` chạy lúc startup: 1 admin, 5 chuyên khoa, 5 bác sĩ, 2 bệnh nhân.
- **Bước 4** — `TaiKhoanController` (Đăng ký / Đăng nhập / Đăng xuất) + cookie claims `VaiTro`, attribute `[Authorize(Roles=...)]`.
- **Bước 5** — `TrangChuController`, `ChuyenKhoaController`, `BacSiController` (tìm + chi tiết), `LichHenController` (đặt / xem / hủy).
- **Bước 6** — `BacSiLichHenController` (xác nhận / từ chối / nhập kết quả khám → `HoSoBenhAn`).
- **Bước 7** — `Controllers/Admin/*` (Dashboard + CRUD) + Chart.js cho thống kê.
- **Bước 8** — `Services/EmailService.cs` dùng MailKit, gửi mail 3 sự kiện + hướng dẫn tạo Gmail App Password.
- **Bước 9** — Layout Bootstrap 5 đẹp hơn, navbar theo vai trò, README.md đầy đủ.
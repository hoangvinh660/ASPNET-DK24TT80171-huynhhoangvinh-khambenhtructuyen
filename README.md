# Hệ thống đặt lịch khám bệnh trực tuyến

Đồ án tốt nghiệp — ASP.NET Core MVC 8 + SQL Server.

**Tác giả:** Huỳnh Hoàng Vinh — DK24TT80171

---

## Giới thiệu

Website đặt lịch khám: bệnh nhân tìm bác sĩ và đặt lịch; bác sĩ xác nhận / nhập kết quả; admin quản trị hệ thống.

| Thành phần | Công nghệ |
|---|---|
| Backend | ASP.NET Core MVC 8 |
| Database | SQL Server + EF Core |
| Giao diện | Bootstrap 5 |
| Email | MailKit (tùy chọn) |

---

## Yêu cầu

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2018+ (Express được)
- (Tùy chọn) Visual Studio 2022 — mở file `src/DatLichKhamBenh.sln`

---

## Cấu trúc thư mục

```
dangkykhambenh/
├── README.md
├── setup/                        # File ho tro (backup DB, ...)
│   └── Database/                 # Backup mau (.bak / .zip)
└── src/
    ├── DatLichKhamBenh.sln
    └── DatLichKhamBenh/          # Source web
        ├── Controllers/
        ├── Models/
        ├── Views/
        ├── Migrations/
        ├── appsettings.json
        └── DatLichKhamBenh.csproj
```

> File `*.ps1` trong `setup/` (nếu có trên máy bạn) **không đưa lên GitHub** — dùng lệnh `dotnet` trong README.

---

## Cách chạy

### 1. Clone và cấu hình SQL Server

```powershell
git clone <url-repo>
cd dangkykhambenh
```

Mở `src/DatLichKhamBenh/appsettings.json`, sửa connection string:

```json
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=TEN-SERVER;Database=DatLichKhamBenh;User Id=sa;Password=MAT-KHAU;TrustServerCertificate=True"
}
```

### 2. Chạy ứng dụng

Tại thư mục gốc repo:

```powershell
dotnet restore src/DatLichKhamBenh/DatLichKhamBenh.csproj
dotnet run --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
```

Lần chạy đầu: EF Core tự tạo bảng và nạp dữ liệu mẫu.

Mở trình duyệt: **http://localhost:5231** (port xem trong `src/DatLichKhamBenh/Properties/launchSettings.json`).

### 3. (Tùy chọn) Mở bằng Visual Studio

Mở `src/DatLichKhamBenh.sln` → nhấn F5.

---

## Tài khoản demo

| Vai trò | Username | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Bác sĩ | `bs.an` | `Bacsi@123` |
| Bệnh nhân | `bn.hoa` | `Benhnhan@123` |

Đăng ký bệnh nhân mới: `/TaiKhoan/DangKy`  
Admin: `/Admin/Dashboard`

---

## Cập nhật / reset database (EF Core)

Cài tool (một lần):

```powershell
dotnet tool install --global dotnet-ef
```

**Tạo migration mới** (sau khi sửa Model):

```powershell
dotnet ef migrations add TenMigration --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
dotnet ef database update --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
```

**Reset database** (xóa hết dữ liệu):

```powershell
dotnet ef database drop --force --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
dotnet ef database update --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
```

Chạy lại `dotnet run` để nạp lại dữ liệu mẫu.

---

## Email (tùy chọn)

Mặc định hệ thống **chỉ ghi log** mail ra console (không cần Gmail).

Muốn gửi thật: cấu hình `EmailSettings` trong `src/DatLichKhamBenh/appsettings.json` (Gmail + App Password). **Không commit mật khẩu thật** lên Git.

---

## Lưu ý khi push GitHub

- Đẩy `src/` và `setup/` (backup trong `setup/Database/`).
- **Không** commit: `*.ps1`, `appsettings.Online.json`, thư mục `backups/` ở root.
- Sửa connection string cho đúng máy của người clone.

---

## Bản quyền

Đồ án tốt nghiệp — chỉ dùng cho mục đích học tập.

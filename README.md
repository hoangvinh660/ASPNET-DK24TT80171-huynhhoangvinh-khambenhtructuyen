# Hệ thống đặt lịch khám bệnh trực tuyến

Đồ án tốt nghiệp — Website đặt lịch khám bệnh sử dụng ASP.NET Core MVC.

> Tác giả: **Huỳnh Hoàng Vinh** — DK24TT80171

---

## 1. Tổng quan

Website cho phép bệnh nhân tìm bác sĩ theo chuyên khoa, đặt lịch khám, theo dõi trạng thái lịch hẹn và nhận email thông báo tự động. Bác sĩ có khu vực riêng để xác nhận / từ chối / nhập kết quả khám. Admin có dashboard thống kê và CRUD đầy đủ cho hệ thống.

### Stack công nghệ

| Lớp | Công nghệ |
|---|---|
| Backend | ASP.NET Core MVC 8 (C#) |
| ORM | Entity Framework Core 8 (Code First) |
| Database | SQL Server 2018+ |
| Frontend | Bootstrap 5, Bootstrap Icons, Chart.js |
| Email | MailKit (Gmail SMTP / StartTLS) |
| Auth | Cookie Authentication + Claims (Role-based) |

### Tính năng chính

- **Bệnh nhân**: đăng ký, đăng nhập, tìm bác sĩ, đặt lịch, xem & hủy lịch hẹn, nhận email xác nhận.
- **Bác sĩ**: xem lịch hẹn đến với mình, xác nhận / từ chối, nhập chẩn đoán + đơn thuốc + lời khuyên (sinh `HoSoBenhAn`).
- **Admin**: dashboard với Chart.js (thống kê theo ngày + trạng thái + Top BS), CRUD chuyên khoa / bác sĩ / người dùng, quản lý toàn bộ lịch hẹn, khóa / mở khóa tài khoản.

---

## 2. Yêu cầu hệ thống

- Windows 10/11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2018+ (Express cũng OK)
- (Tùy chọn) [Visual Studio 2022](https://visualstudio.microsoft.com/) hoặc VS Code
- (Tùy chọn) Tài khoản Gmail + App Password để bật gửi email thật

---

## 3. Cấu trúc thư mục

```
dangkykhambenh/
├── DatLichKhamBenh.sln
├── README.md
├── .gitignore
└── DatLichKhamBenh/
    ├── Controllers/
    │   ├── Admin/                       # 5 controller Admin (dashboard + CRUD)
    │   ├── BacSiController.cs           # Tìm/xem bác sĩ (public)
    │   ├── BacSiLichHenController.cs    # Khu vực bác sĩ: xác nhận, từ chối, nhập kết quả
    │   ├── ChuyenKhoaController.cs      # Danh sách / chi tiết chuyên khoa (public)
    │   ├── LichHenController.cs         # Bệnh nhân: đặt / xem / hủy lịch
    │   ├── TaiKhoanController.cs        # Đăng ký, đăng nhập, đăng xuất
    │   └── TrangChuController.cs        # Trang chủ + Privacy + Error
    ├── Models/
    │   ├── AppDbContext.cs              # DbContext + cấu hình quan hệ
    │   ├── SeedData.cs                  # Nạp dữ liệu mẫu lúc startup
    │   ├── NguoiDung.cs                 # Tài khoản chung cho 3 vai trò
    │   ├── BacSi.cs / BenhNhan.cs       # Hồ sơ liên kết 1-1 với NguoiDung
    │   ├── ChuyenKhoa.cs / LichHen.cs / HoSoBenhAn.cs
    │   └── ViewModels/                  # Các form input (đăng ký, đăng nhập, đặt lịch, …)
    ├── Services/
    │   ├── IEmailService.cs
    │   ├── EmailService.cs              # MailKit + chế độ "dev log" fallback
    │   └── EmailSettings.cs
    ├── Migrations/                      # EF Core migrations
    ├── Views/                            # Razor views (theo controller)
    │   ├── Shared/_Layout.cshtml         # Layout chính (cho user/BN/BS)
    │   ├── Shared/_AdminLayout.cshtml    # Layout cho khu Admin (sidebar)
    │   └── …
    ├── wwwroot/                          # Static (css, js, lib bootstrap, jQuery)
    ├── Program.cs                        # Đăng ký DI, Auth, Session, Email
    ├── appsettings.json
    └── DatLichKhamBenh.csproj
```

---

## 4. Cách chạy lần đầu

### 4.1 Cấu hình connection string

Mở `DatLichKhamBenh/appsettings.json`, sửa `ConnectionStrings.DatLichKhamBenh` cho khớp với SQL Server của bạn:

```jsonc
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=YOUR-SERVER;Database=DatLichKhamBenh;User Id=sa;Password=YOUR-PASS;TrustServerCertificate=True"
}
```

> Mặc định đang dùng `Server=DESKTOP-IL8JOMQ\\BARTENDER` — đổi sang server của bạn.

### 4.2 Restore + chạy

```powershell
dotnet restore
dotnet build
dotnet run --project DatLichKhamBenh\DatLichKhamBenh.csproj
```

Khi khởi động lần đầu:
- EF Core sẽ tự `Migrate()` để tạo các bảng.
- `SeedData.Initialize()` sẽ nạp dữ liệu mẫu (xem mục **5. Tài khoản demo**).

Mặc định ứng dụng chạy ở `http://localhost:5231` (port có thể khác — xem `Properties/launchSettings.json`). Mở trình duyệt là dùng được.

### 4.3 Script tiện ích PowerShell

Chạy trong **PowerShell** tại thư mục gốc repo (`dangkykhambenh/`). Lần đầu có thể cần:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

| Script | Tác dụng |
|---|---|
| `chay-web.ps1` | Build + chạy nhanh project |
| `cap-nhat-database.ps1` | Tạo migration mới rồi `database update` |
| `reset-database.ps1` | **Drop** database rồi tạo lại từ đầu (xóa toàn bộ dữ liệu) |

**Ví dụ:**

```powershell
# Chạy web (mặc định http://localhost:5231)
.\chay-web.ps1

# Bỏ qua build nếu vừa build xong
.\chay-web.ps1 -NoBuild

# Sau khi sửa Model, tạo migration + cập nhật DB
.\cap-nhat-database.ps1 -TenMigration "ThemCotMoi"
# hoặc chạy .\cap-nhat-database.ps1 rồi nhập tên khi được hỏi

# Reset DB về trạng thái ban đầu (mất hết dữ liệu — gõ yes để xác nhận)
.\reset-database.ps1
```

> Trước khi chạy: sửa `ConnectionStrings` trong `DatLichKhamBenh/appsettings.json` cho khớp SQL Server của bạn.

---

## 5. Tài khoản demo (mật khẩu lưu plain text — chỉ dùng cho đồ án)

| Vai trò | Username | Mật khẩu | Ghi chú |
|---|---|---|---|
| Admin | `admin` | `Admin@123` | Truy cập `/Admin/Dashboard` |
| Bác sĩ | `bs.an` | `Bacsi@123` | Nội tổng quát |
| Bác sĩ | `bs.binh` | `Bacsi@123` | Ngoại tổng quát |
| Bác sĩ | `bs.cuong` | `Bacsi@123` | Sản phụ khoa |
| Bác sĩ | `bs.dung` | `Bacsi@123` | Nhi khoa |
| Bác sĩ | `bs.em` | `Bacsi@123` | Mắt |
| Bệnh nhân | `bn.hoa` | `Benhnhan@123` | Vũ Thị Hoa |
| Bệnh nhân | `bn.minh` | `Benhnhan@123` | Đỗ Văn Minh |

Đăng ký tài khoản mới (chỉ tạo bệnh nhân) tại `/TaiKhoan/DangKy`.

---

## 6. Lộ trình test end-to-end (15 phút)

1. **Đăng ký bệnh nhân mới** tại `/TaiKhoan/DangKy` *(hoặc dùng `bn.hoa`)*.
2. Vào **Bác sĩ → Tìm bác sĩ** → chọn 1 bác sĩ → **Đặt lịch** ngày mai, giờ bất kỳ.
3. Vào **Lịch hẹn của tôi** — thấy trạng thái *Chờ xác nhận*. Quan sát log server có dòng `[EMAIL-DEV] ... Da nhan lich hen #X`.
4. **Đăng xuất**, đăng nhập bằng bác sĩ tương ứng (vd `bs.an`) → menu *Lịch hẹn của tôi* → **Xác nhận**. Log server có `[EMAIL-DEV] ... Lich hen #X da duoc xac nhan`.
5. Quay lại bác sĩ → **Nhập kết quả** → ghi chẩn đoán + đơn thuốc → Lưu. Trạng thái chuyển sang *Đã khám*, tạo `HoSoBenhAn`.
6. **Đăng nhập admin** → vào **Dashboard** — thấy số liệu cập nhật + biểu đồ.
7. Admin → **Lịch hẹn** lọc theo trạng thái / bác sĩ / ngày. Có thể **Hủy** lịch chưa khám.
8. Admin → **Người dùng** → khóa tạm tài khoản `bn.hoa` → đăng xuất → thử đăng nhập `bn.hoa` (sẽ bị chặn) → mở khóa lại.
9. Admin → **Chuyên khoa** + **Bác sĩ**: thêm 1 chuyên khoa mới → thêm 1 bác sĩ thuộc chuyên khoa đó (form sẽ tạo cả tài khoản đăng nhập cho bác sĩ).

---

## 7. Cấu hình gửi mail qua Gmail

Hệ thống dùng `MailKit` gửi email cho 3 sự kiện chính:

| Sự kiện | Action gọi | Người nhận |
|---|---|---|
| Bệnh nhân đặt lịch | `LichHenController.DatLich` (POST) | Bệnh nhân |
| Bác sĩ xác nhận lịch | `BacSiLichHenController.XacNhan` | Bệnh nhân |
| Lịch bị hủy (BN/BS/Admin) | `LichHen/Huy`, `BacSiLichHen/TuChoi`, `AdminLichHen/Huy` | Bệnh nhân |

### Chế độ "dev log" (mặc định)

Khi `EmailSettings.SenderEmail` / `SenderPassword` còn là **placeholder** trong `appsettings.json`, service **KHÔNG kết nối SMTP** mà chỉ **log ra console** nội dung mail. Tiện để demo offline / test mà không cần Gmail thật.

```
warn: DatLichKhamBenh.Services.EmailService[0]
      [EMAIL-DEV] EmailSettings chua duoc cau hinh -> KHONG gui mail. To: ...
```

### Bật gửi thật (Gmail SMTP + App Password)

1. Đăng nhập Gmail → bật **Xác minh 2 bước** tại <https://myaccount.google.com/security>.
2. Truy cập **App passwords** tại <https://myaccount.google.com/apppasswords>.
3. Chọn *App* = "Mail", *Device* = "Other" → đặt tên `DatLichKhamBenh` → **Generate**.
4. Google trả về mật khẩu **16 ký tự** dạng `xxxx yyyy zzzz wwww`. Bỏ khoảng trắng khi paste.
5. Sửa `DatLichKhamBenh/appsettings.json`:

```jsonc
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderName": "Phòng khám DatLichKhamBenh",
  "SenderEmail": "email-cua-ban@gmail.com",
  "SenderPassword": "xxxxyyyyzzzzwwww"
}
```

6. Restart project. Khi đặt lịch / xác nhận / hủy lịch, log sẽ hiện:

```
info: DatLichKhamBenh.Services.EmailService[0]
      [EMAIL] Da gui mail toi hoa.vu@gmail.com - [DatLichKhamBenh] Da nhan lich hen #X
```

### Lưu ý bảo mật

- **KHÔNG commit** `SenderPassword` thật lên Git. Trong môi trường thật, dùng biến môi trường `EmailSettings__SenderPassword` hoặc User Secrets.
- App Password chỉ hoạt động khi tài khoản đã bật 2-Step Verification.
- Nếu dùng SMTP khác (Outlook, mailtrap.io…), chỉ cần đổi `SmtpServer` / `Port` cho phù hợp.

---

## 8. Reset database

```powershell
.\reset-database.ps1
```

Hoặc thủ công:

```powershell
dotnet ef database drop --force --project DatLichKhamBenh\DatLichKhamBenh.csproj
dotnet ef database update --project DatLichKhamBenh\DatLichKhamBenh.csproj
```

Sau khi DB rỗng, lần chạy `dotnet run` tiếp theo `SeedData.Initialize()` sẽ nạp lại dữ liệu mẫu.

---

## 9. Sơ đồ dữ liệu (6 bảng chính)

```
NguoiDung 1 ─── 1 BacSi   ───┐
          1 ─── 1 BenhNhan ──┤
                             │
                  ChuyenKhoa 1 ─── N BacSi
                  BenhNhan  1 ─── N LichHen
                  BacSi     1 ─── N LichHen
                  LichHen   1 ─── 1 HoSoBenhAn (sau khi khám)
```

- `NguoiDung.VaiTro ∈ { "Admin", "BacSi", "BenhNhan" }`
- `LichHen.TrangThai ∈ { "ChoXacNhan", "DaXacNhan", "DaKham", "DaHuy" }`
- Unique index: `(TenDangNhap)`, `(Email)`, `(MaBacSi, NgayKham, GioKham)` — chặn trùng khung giờ.

---

## 10. Bản quyền

Đồ án tốt nghiệp — chỉ dùng cho mục đích học tập.

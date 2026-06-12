# Hướng dẫn cài đặt chi tiết — Đặt lịch khám bệnh trực tuyến

Tài liệu này hướng dẫn từng bước cài đặt môi trường và chạy dự án **ASP.NET Core MVC 8 + SQL Server** trên Windows.

---

## Mục lục

1. [Tổng quan](#1-tổng-quan)
2. [Yêu cầu hệ thống](#2-yêu-cầu-hệ-thống)
3. [Tải mã nguồn](#3-tải-mã-nguồn)
4. [Cài đặt .NET 8 SDK](#4-cài-đặt-net-8-sdk)
5. [Cài đặt Microsoft SQL Server](#5-cài-đặt-microsoft-sql-server)
6. [Cài đặt SQL Server Management Studio (SSMS)](#6-cài-đặt-sql-server-management-studio-ssms)
7. [Cấu hình SQL Server sau khi cài](#7-cấu-hình-sql-server-sau-khi-cài)
8. [Tạo database](#8-tạo-database)
9. [Cấu hình connection string](#9-cấu-hình-connection-string)
10. [Cài đặt Visual Studio (tùy chọn)](#10-cài-đặt-visual-studio-tùy-chọn)
11. [Chạy dự án](#11-chạy-dự-án)
12. [Tài khoản demo](#12-tài-khoản-demo)
13. [Cấu hình gửi email (tùy chọn)](#13-cấu-hình-gửi-email-tùy-chọn)
14. [Xử lý lỗi thường gặp](#14-xử-lý-lỗi-thường-gặp)

---

## 1. Tổng quan

| Thành phần | Phiên bản / Ghi chú |
|---|---|
| Framework | ASP.NET Core MVC **.NET 8** |
| Database | **SQL Server 2018+** (khuyến nghị SQL Server 2022 Express) |
| ORM | Entity Framework Core 8 |
| Cổng mặc định | `http://localhost:5231` |

Khi chạy lần đầu, ứng dụng sẽ **tự tạo bảng** (EF Core Migration) và **nạp dữ liệu mẫu** nếu database còn trống. Bạn cũng có thể import sẵn file SQL/backup trong thư mục `setup/Database/`.

---

## 2. Yêu cầu hệ thống

- **Hệ điều hành:** Windows 10/11 (64-bit)
- **RAM:** Tối thiểu 4 GB (khuyến nghị 8 GB trở lên khi chạy SSMS + Visual Studio)
- **Ổ cứng:** ~10 GB trống (SQL Server + .NET + Visual Studio)
- **Quyền:** Tài khoản Windows có quyền Administrator (để cài SQL Server)

---

## 3. Tải mã nguồn

### Cách 1: Clone bằng Git

```powershell
git clone https://github.com/hoangvinh660/ASPNET-DK24TT80171-huynhhoangvinh-khambenhtructuyen.git
cd ASPNET-DK24TT80171-huynhhoangvinh-khambenhtructuyen
```

### Cách 2: Tải ZIP từ GitHub

1. Vào trang repository trên GitHub.
2. Chọn **Code** → **Download ZIP**.
3. Giải nén và mở thư mục dự án.

### Cấu trúc thư mục quan trọng

```
├── src/DatLichKhamBenh/          # Mã nguồn web
│   ├── appsettings.json          # Connection string, cấu hình email
│   └── DatLichKhamBenh.csproj    # Project .NET 8
├── setup/
│   ├── Install/                  # Tài liệu cài đặt (file này)
│   └── Database/                 # Backup SQL / file .bak
└── thesis/                       # Tài liệu đồ án
```

---

## 4. Cài đặt .NET 8 SDK

### Bước 4.1 — Tải SDK

1. Truy cập: https://dotnet.microsoft.com/download/dotnet/8.0
2. Tải **.NET 8.0 SDK** (không phải Runtime) cho Windows x64.
3. Chạy file cài đặt và làm theo hướng dẫn (Next → Install → Finish).

### Bước 4.2 — Kiểm tra

Mở **PowerShell** hoặc **CMD** mới và chạy:

```powershell
dotnet --version
```

Kết quả mong đợi: `8.0.xxx` (ví dụ `8.0.404`).

Nếu lệnh không được nhận diện, khởi động lại máy hoặc đăng xuất/đăng nhập lại Windows.

---

## 5. Cài đặt Microsoft SQL Server

Khuyến nghị dùng **SQL Server 2022 Express** — miễn phí, đủ cho môi trường học tập và demo.

### Bước 5.1 — Tải SQL Server Express

1. Truy cập: https://www.microsoft.com/sql-server/sql-server-downloads
2. Chọn **Express** → **Download now**.
3. Chọn gói **Express Advanced** hoặc **Basic** (cả hai đều được; Advanced có thêm SSMS trong một số bộ cài).

### Bước 5.2 — Chạy trình cài đặt

1. Chọn loại cài đặt: **Basic** (đơn giản) hoặc **Custom** (tùy chỉnh chi tiết hơn).

#### Nếu chọn Custom (khuyến nghị cho đồ án)

1. Chọn **New SQL Server stand-alone installation**.
2. Chấp nhận điều khoản license.
3. Bỏ qua (hoặc cài) **Azure Extension for SQL Server** — không bắt buộc.
4. **Feature Selection** — chọn tối thiểu:
   - **Database Engine Services**
   - (Tùy chọn) **SQL Server Replication**, **Full-Text and Semantic Extractions for Search**
5. **Instance Configuration:**
   - Chọn **Named instance** → đặt tên `SQLEXPRESS` (mặc định, dễ nhớ).
   - Ghi nhớ tên instance — dùng trong connection string dạng `TEN-MAY\SQLEXPRESS`.
6. **Server Configuration:** giữ mặc định (SQL Server Database Engine chạy dưới tài khoản `NT Service\MSSQL$SQLEXPRESS`).
7. **Database Engine Configuration** — **quan trọng:**
   - Chọn **Mixed Mode (SQL Server authentication and Windows authentication)**.
   - Đặt mật khẩu cho tài khoản **sa** (ví dụ: `Admin@123`) — **ghi nhớ mật khẩu này**.
   - Thêm tài khoản Windows hiện tại của bạn vào danh sách **SQL Server administrators** (nút **Add Current User**).
8. Hoàn tất cài đặt → **Close**.

### Bước 5.3 — Kiểm tra dịch vụ SQL Server đang chạy

Mở **PowerShell**:

```powershell
Get-Service | Where-Object { $_.Name -like "MSSQL*" }
```

Trạng thái **Running** nghĩa là SQL Server đã sẵn sàng.

Ví dụ tên dịch vụ:
- Instance mặc định: `MSSQLSERVER`
- Instance tên `SQLEXPRESS`: `MSSQL$SQLEXPRESS`

### Bước 5.4 — Xác định tên Server để kết nối

| Loại cài đặt | Tên server thường dùng |
|---|---|
| Named instance `SQLEXPRESS` | `localhost\SQLEXPRESS` hoặc `.\SQLEXPRESS` |
| Default instance | `localhost` hoặc `.` |
| Máy khác trong mạng LAN | `TEN-MAY\SQLEXPRESS` |

Lấy tên máy Windows:

```powershell
hostname
```

Ví dụ máy tên `DESKTOP-ABC`, instance `SQLEXPRESS` → server là `DESKTOP-ABC\SQLEXPRESS`.

---

## 6. Cài đặt SQL Server Management Studio (SSMS)

SSMS dùng để quản lý database, chạy script SQL, restore backup.

1. Truy cập: https://aka.ms/ssmsfullsetup
2. Tải và cài đặt SSMS (Next → Install → khởi động lại nếu được yêu cầu).
3. Mở **SQL Server Management Studio**.
4. Cửa sổ **Connect to Server:**
   - **Server type:** Database Engine
   - **Server name:** `localhost\SQLEXPRESS` (hoặc tên server của bạn)
   - **Authentication:** Windows Authentication (lần đầu) hoặc SQL Server Authentication (`sa` + mật khẩu)
5. Nhấn **Connect**.

Nếu kết nối thành công, SQL Server đã cài đúng.

---

## 7. Cấu hình SQL Server sau khi cài

### 7.1 — Bật SQL Server Authentication (nếu lúc cài chọn Windows Authentication only)

1. Trong SSMS, chuột phải vào server (cây bên trái) → **Properties**.
2. Tab **Security** → chọn **SQL Server and Windows Authentication mode**.
3. **OK** → khởi động lại dịch vụ SQL Server:

```powershell
Restart-Service "MSSQL$SQLEXPRESS"
```

(Đổi tên dịch vụ nếu instance khác.)

4. Mở rộng **Security** → **Logins** → chuột phải **sa** → **Properties**.
5. Đặt mật khẩu mới, bỏ tick **Enforce password policy** (nếu chỉ dùng local), tab **Status** → **Login: Enabled** → **OK**.

### 7.2 — Bật giao thức TCP/IP (khi kết nối từ ứng dụng bị lỗi network)

1. Mở **SQL Server Configuration Manager** (tìm trong Start Menu).
2. **SQL Server Network Configuration** → **Protocols for SQLEXPRESS** (hoặc `MSSQLSERVER`).
3. Chuột phải **TCP/IP** → **Enable**.
4. Khởi động lại dịch vụ SQL Server.

### 7.3 — Mở firewall (khi cần truy cập từ máy khác)

Chỉ cần nếu chạy web trên máy A, SQL trên máy B. Trên máy chạy SQL:

```powershell
New-NetFirewallRule -DisplayName "SQL Server" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
```

---

## 8. Tạo database

Có **3 cách**. Chọn **một** cách phù hợp.

### Cách A — Tự động (khuyến nghị, đơn giản nhất)

**Không cần tạo database thủ công.** Chỉ cần:

1. Cấu hình connection string (mục 9) với tên database `DatLichKhamBenh`.
2. Chạy ứng dụng (mục 11).

Ứng dụng sẽ gọi `db.Database.Migrate()` và `SeedData.Initialize()` — tự tạo database, bảng, và dữ liệu mẫu (admin, bác sĩ, bệnh nhân, chuyên khoa...).

> **Lưu ý:** Tài khoản trong connection string phải có quyền **tạo database** (tài khoản `sa` hoặc Windows admin).

---

### Cách B — Import file SQL script

Dùng khi muốn dữ liệu giống bản backup ngày 09/06/2026.

1. Giải nén (nếu cần): `setup/Database/DatLichKhamBenh_20260609_141809.sql.zip`
2. Mở SSMS → **File** → **Open** → **File** → chọn `setup/Database/DatLichKhamBenh_20260609_141809.sql`
3. Nhấn **Execute** (F5) và đợi script chạy xong.
4. Kiểm tra: trong **Object Explorer** → **Databases** → có `DatLichKhamBenh`.

---

### Cách C — Restore file backup (.bak)

1. Giải nén: `setup/Database/DatLichKhamBenh_20260527_141344.bak.zip`
2. Trong SSMS, chuột phải **Databases** → **Restore Database...**
3. Chọn **Device** → **Add** → trỏ tới file `.bak` đã giải nén.
4. Tab **Options** → tick **Overwrite the existing database (WITH REPLACE)** nếu database đã tồn tại.
5. **OK** để restore.

---

## 9. Cấu hình connection string

Mở file:

```
src/DatLichKhamBenh/appsettings.json
```

Sửa mục `ConnectionStrings` → `DatLichKhamBenh` cho khớp SQL Server trên máy bạn.

### Ví dụ 1 — SQL Server Authentication (tài khoản `sa`)

```json
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=localhost\\SQLEXPRESS;Database=DatLichKhamBenh;User Id=sa;Password=Admin@123;TrustServerCertificate=True"
}
```

### Ví dụ 2 — Windows Authentication (không cần user/password)

```json
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=localhost\\SQLEXPRESS;Database=DatLichKhamBenh;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### Ví dụ 3 — Instance mặc định, máy tên DESKTOP-ABC

```json
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=DESKTOP-ABC;Database=DatLichKhamBenh;User Id=sa;Password=Admin@123;TrustServerCertificate=True"
}
```

### Giải thích các tham số

| Tham số | Ý nghĩa |
|---|---|
| `Server` | Tên máy + tên instance SQL (ví dụ `localhost\SQLEXPRESS`) |
| `Database` | Tên database — dùng `DatLichKhamBenh` |
| `User Id` / `Password` | Tài khoản SQL Server (khi dùng SQL Authentication) |
| `Trusted_Connection=True` | Dùng tài khoản Windows đang đăng nhập |
| `TrustServerCertificate=True` | Bỏ qua kiểm tra chứng chỉ SSL (phù hợp môi trường local) |

> Trong file JSON, dấu `\` trong tên instance phải viết **hai lần**: `\\` (ví dụ `localhost\\SQLEXPRESS`).

---

## 10. Cài đặt Visual Studio (tùy chọn)

Không bắt buộc — có thể chạy hoàn toàn bằng dòng lệnh `dotnet run`. Visual Studio tiện hơn khi debug và chỉnh sửa code.

1. Tải **Visual Studio 2022 Community** (miễn phí): https://visualstudio.microsoft.com/vs/
2. Trong **Workloads**, chọn:
   - **ASP.NET and web development**
   - **.NET desktop development** (tùy chọn)
3. Trong **Individual components**, đảm bảo có **.NET 8.0 Runtime**.
4. Cài đặt và mở solution:

```
src/DatLichKhamBenh.sln
```

---

## 11. Chạy dự án

### Cách 1 — Dòng lệnh (PowerShell)

Di chuyển vào thư mục gốc dự án (nơi có thư mục `src`):

```powershell
cd đường-dẫn-tới-thư-mục-dự-án
dotnet restore src/DatLichKhamBenh/DatLichKhamBenh.csproj
dotnet run --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
```

Khi thấy dòng tương tự:

```
Now listening on: http://localhost:5231
```

Mở trình duyệt: **http://localhost:5231**

### Cách 2 — Visual Studio

1. Mở `src/DatLichKhamBenh.sln`.
2. Chọn profile **http** (hoặc **https**) trên thanh toolbar.
3. Nhấn **F5** (Run) hoặc **Ctrl+F5** (Run without debugging).

### Các profile chạy (launchSettings.json)

| Profile | URL | Môi trường |
|---|---|---|
| `http` | http://localhost:5231 | Development |
| `https` | https://localhost:7140 + http://5231 | Development |
| `Online` | http://localhost:5231 | Online (giả lập hosting) |

### Dừng ứng dụng

Trong terminal: nhấn **Ctrl+C**.

---

## 12. Tài khoản demo

Sau khi seed/import database thành công, dùng các tài khoản sau để đăng nhập tại `/TaiKhoan/DangNhap`:

| Vai trò | Username | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Bác sĩ | `bs.an` | `Bacsi@123` |
| Bệnh nhân | `bn.hoa` | `Benhnhan@123` |

Các tài khoản bác sĩ khác: `bs.binh`, `bs.cuong`, `bs.dung`, `bs.em` — mật khẩu chung `Bacsi@123`.

Bệnh nhân khác: `bn.minh` — mật khẩu `Benhnhan@123`.

---

## 13. Cấu hình gửi email (tùy chọn)

Chức năng gửi email xác nhận lịch hẹn dùng **Gmail SMTP**. Không cấu hình vẫn chạy được web; chỉ tính năng gửi mail sẽ không hoạt động.

### Cách 1 — Sửa `appsettings.json` (lần chạy đầu, trước khi có dữ liệu trong DB)

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderName": "Phong kham DatLichKhamBenh",
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "app-password-16-ky-tu"
}
```

### Cách 2 — Cấu hình trên giao diện Admin (sau khi đăng nhập admin)

Vào **Admin → Email** (`/Admin/Email`) để bật/tắt và cập nhật SMTP.

### Tạo App Password Gmail

1. Bật **Xác minh 2 bước** cho tài khoản Google.
2. Vào https://myaccount.google.com/apppasswords
3. Tạo App Password cho **Mail** → copy chuỗi 16 ký tự vào `SenderPassword`.

---

## 14. Xử lý lỗi thường gặp

### Lỗi: `A network-related or instance-specific error occurred`

**Nguyên nhân:** Sai tên server, SQL Server chưa chạy, hoặc TCP/IP chưa bật.

**Cách xử lý:**
1. Kiểm tra dịch vụ SQL đang **Running** (mục 5.3).
2. Thử kết nối bằng SSMS với cùng tên server.
3. Bật TCP/IP (mục 7.2).
4. Kiểm tra lại `Server=` trong connection string.

---

### Lỗi: `Login failed for user 'sa'`

**Nguyên nhân:** Sai mật khẩu, chưa bật Mixed Mode, hoặc tài khoản `sa` bị disabled.

**Cách xử lý:** Làm theo mục 7.1 — bật Mixed Mode và enable login `sa`.

---

### Lỗi: `Cannot open database "DatLichKhamBenh"`

**Nguyên nhân:** Database chưa tồn tại và tài khoản không có quyền tạo DB.

**Cách xử lý:**
- Dùng **Cách A** (chạy app với quyền `sa`), hoặc
- Tạo database thủ công trong SSMS:

```sql
CREATE DATABASE DatLichKhamBenh;
```

---

### Lỗi: `dotnet` is not recognized

**Cách xử lý:** Cài lại .NET 8 SDK (mục 4), khởi động lại terminal hoặc máy.

---

### Lỗi port 5231 đã được sử dụng

**Cách xử lý:** Đổi port khi chạy:

```powershell
dotnet run --project src/DatLichKhamBenh/DatLichKhamBenh.csproj --urls "http://localhost:5500"
```

Hoặc sửa `applicationUrl` trong `src/DatLichKhamBenh/Properties/launchSettings.json`.

---

### Ứng dụng chạy nhưng không có dữ liệu / không đăng nhập được

**Nguyên nhân:** Database đã có bảng nhưng bảng `NguoiDungs` trống (seed chỉ chạy khi chưa có user).

**Cách xử lý:**
- Import lại script SQL (Cách B), hoặc
- Xóa database và chạy lại app để seed tự động:

```sql
USE master;
DROP DATABASE DatLichKhamBenh;
```

Sau đó `dotnet run` lại.

---

### Lỗi migration khi đã import SQL/restore .bak

Nếu đã import đủ schema qua script/backup, migration thường vẫn tương thích vì bảng `__EFMigrationsHistory` đã có sẵn trong file backup. Nếu gặp lỗi migration, kiểm tra bảng này trong SSMS có đủ 3 bản ghi migration hay không.

---

## Checklist nhanh

- [ ] Đã cài **.NET 8 SDK** (`dotnet --version` → 8.0.x)
- [ ] Đã cài **SQL Server** và dịch vụ đang **Running**
- [ ] Đã cài **SSMS** và kết nối được tới server
- [ ] Đã bật **Mixed Mode** và mật khẩu `sa` (nếu dùng SQL Authentication)
- [ ] Đã sửa **connection string** trong `appsettings.json`
- [ ] Đã chạy `dotnet run` và mở **http://localhost:5231**
- [ ] Đăng nhập thử bằng `admin` / `Admin@123`

---

*Tài liệu thuộc đồ án tốt nghiệp — Huỳnh Hoàng Vinh, DK24TT80171.*

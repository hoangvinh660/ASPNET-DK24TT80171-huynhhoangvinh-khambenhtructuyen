# Đặt lịch khám bệnh trực tuyến

Đồ án tốt nghiệp — ASP.NET Core MVC 8 + SQL Server.

**Sinh viên:** Huỳnh Hoàng Vinh — DK24TT80171  
**Giảng viên hướng dẫn:** TS. Đoàn Phước Miền

---

## Demo online

**https://huynhvinh-001-site1.anytempurl.com/**

Website demo đang chạy trên hosting. Dùng tài khoản ở mục bên dưới để đăng nhập thử.

> Website tồn tại đến **30/06/2026**.

---

## Tải về

```powershell
git clone https://github.com/hoangvinh660/ASPNET-DK24TT80171-huynhhoangvinh-khambenhtructuyen.git
cd ASPNET-DK24TT80171-huynhhoangvinh-khambenhtructuyen
```

---

## Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2018+ (Express được)

---

## Chạy project

**1.** Sửa connection string trong `src/DatLichKhamBenh/appsettings.json`:

```json
"ConnectionStrings": {
  "DatLichKhamBenh": "Server=TEN-SERVER;Database=DatLichKhamBenh;User Id=sa;Password=MAT-KHAU;TrustServerCertificate=True"
}
```

**2.** Chạy lệnh:

```powershell
dotnet run --project src/DatLichKhamBenh/DatLichKhamBenh.csproj
```

**3.** Mở trình duyệt: http://localhost:5231

Lần chạy đầu, hệ thống tự tạo database và nạp dữ liệu mẫu.

---

## Tài khoản demo

| Vai trò | Username | Mật khẩu |
|---|---|---|
| Admin | `admin` | `Admin@123` |
| Bác sĩ | `bs.an` | `Bacsi@123` |
| Bệnh nhân | `bn.hoa` | `Benhnhan@123` |

---

## Cấu trúc repo

```
├── src/DatLichKhamBenh/     # Source web
├── setup/Database/          # Backup database mẫu
└── thesis/                  # Tài liệu đồ án
```

Mở bằng Visual Studio: `src/DatLichKhamBenh.sln`

---

*Đồ án tốt nghiệp — chỉ dùng cho mục đích học tập.*

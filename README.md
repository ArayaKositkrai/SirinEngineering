# SirinEngineering

ระบบจัดการร้าน/POS สำหรับ SirinEngineering พัฒนาด้วย ASP.NET Core MVC (.NET 10)

## Technology Stack

- **Backend:** ASP.NET Core MVC (.NET 10.0)
- **ORM:** Entity Framework Core 9.0 + Pomelo.EntityFrameworkCore.MySql
- **Database:** MySQL
- **Frontend:** Razor Views (.cshtml), Bootstrap, jQuery, jQuery Validation
- **Auth:** Cookie Authentication (`MyCookieAuth`) + Session

## วิธีเปิดโปรเจกต์

### 1. ติดตั้งเครื่องมือที่ต้องใช้

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- MySQL Server (เช่น MySQL, MariaDB, หรือ XAMPP) ที่รันอยู่ที่ `localhost:3305`

### 2. ตั้งค่าฐานข้อมูล

ตรวจสอบ/แก้ไข connection string ที่ [appsettings.json](appsettings.json) ให้ตรงกับเครื่องของคุณ:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3305;Database=projectdb;User=root;Password=1245;SslMode=None;;AllowPublicKeyRetrieval=True;"
}
```

จากนั้นสร้างฐานข้อมูลด้วย EF Core migrations:

```bash
dotnet ef database update
```

### 3. รันโปรเจกต์

```bash
dotnet restore
dotnet run
```

เปิดเบราว์เซอร์ไปที่ URL ที่ปรากฏใน terminal (เช่น `https://localhost:xxxx`) frontend และ backend รันรวมอยู่ในโปรเซสเดียวกัน (ไม่ต้องรันแยก)

## บัญชีผู้ใช้และสิทธิ์ (Role)

ระบบแบ่งสิทธิ์ผู้ใช้ตาม `U_RoleID` ในตาราง `TBL_User`:

| RoleID | Role     | หน้าแรกหลัง Login       |
|--------|----------|--------------------------|
| 1      | Admin    | `/Admin/Dashboard`       |
| 2      | Staff    | `/Sales/Shop`            |
| 3      | Customer | `/Product/Index`         |

### บัญชีทดสอบ

| Role  | Username | Password |
|-------|----------|----------|
| Admin | `Admin`  | `Admin`  |
| Staff | `Staff`  | `Staff`  |

> หมายเหตุ: ต้องมีผู้ใช้เหล่านี้อยู่ในตาราง `TBL_User` ก่อนจึงจะ Login ได้ หากฐานข้อมูลยังไม่มีข้อมูล สามารถสร้างได้ด้วยคำสั่ง SQL ต่อไปนี้ (รหัสผ่านเก็บเป็น plain text ตาม logic ปัจจุบันของ [AccountController.cs](Controllers/AccountController.cs)):

```sql
INSERT INTO TBL_User (U_Username, U_Password, U_FullName, U_RoleID, U_IsActive)
VALUES ('Admin', 'Admin', 'Administrator', 1, 1);

INSERT INTO TBL_User (U_Username, U_Password, U_FullName, U_RoleID, U_IsActive)
VALUES ('Staff', 'Staff', 'Staff', 2, 1);
```

### การเข้าใช้งาน

1. ไปที่หน้า Login (`/Account/Login`)
2. กรอก Username/Password ตามตารางด้านบน
3. ระบบจะตรวจสอบ `U_Username`, `U_Password` และ `U_IsActive = true` ในตาราง `TBL_User` แล้ว redirect ไปยังหน้าตาม Role โดยอัตโนมัติ

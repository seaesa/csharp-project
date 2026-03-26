# FarmNet — Hệ thống quản lý quy trình sản xuất nông nghiệp

## Yêu cầu

| Công cụ    | Phiên bản tối thiểu             |
| ---------- | ------------------------------- |
| .NET SDK   | 10.0                            |
| SQL Server | 2019+ (hoặc SQL Server Express) |
| Ganache    |                                 |

---

## Cài đặt & Chạy

### 1. Clone & restore

```bash
git clone <repo-url>
cd farm-net
dotnet restore
dotnet tool restore
```

### 2. Cấu hình database

Mở `src/FarmNet.Api/appsettings.json` và chỉnh connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FarmNetDb;User Id=sa;Password=<YOUR_PASSWORD>;TrustServerCertificate=True;"
}
```

### 3. Chạy migration

```bash
dotnet tool run dotnet-ef database update --project src/FarmNet.Infrastructure --startup-project src/FarmNet.Api
```

### 4. Chạy API

```bash
dotnet run --project src/FarmNet.Api
# API: http://localhost:5045
# Scalar API docs: http://localhost:5045/scalar/v1
```

### 5. Chạy Frontend

```bash
dotnet run --project src/FarmNet.Client
# Client: http://localhost:5260
```

---

## Tài khoản mặc định

Được tạo tự động khi chạy lần đầu:

| Vai trò   | Email             | Mật khẩu    |
| --------- | ----------------- | ----------- |
| **Admin** | `admin@gmail.com` | `admin@123` |

### Cài đặt Ganache (Test Local Blockchain)

**Ganache UI** tại [trufflesuite.com/ganache](https://trufflesuite.com/ganache/).

### Lấy private key từ Ganache

1. Mở Ganache → tab **Accounts**
2. Click biểu tượng 🔑 bên cạnh bất kỳ account nào → copy private key

### Cấu hình `appsettings.json`

```json
"Blockchain": {
  "RpcUrl": "http://127.0.0.1:7545",
  "PrivateKey": "PRIVATE_KEY_TỪ_GANACHE"
}
```

## Đẩy dữ liệu cảm biến (IoT API)

```http
POST http://localhost:5045/api/sensors/data
Content-Type: application/json

{
  "device": "ESP32_FARM_001",
  "batchID": "XOAI001",
  "time": "2026-03-25 14:30:05",
  "temp": 30.5,
  "hum": 65.2,
  "rain": 1,
  "water": 1800,
  "gas": 120,
  "pump": 0
}

```

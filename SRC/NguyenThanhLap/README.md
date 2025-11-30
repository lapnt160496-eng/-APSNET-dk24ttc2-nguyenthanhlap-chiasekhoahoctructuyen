# 🎓 Website Khóa Học Trực Tuyến - ASP.NET MVC

Nền tảng học trực tuyến hoàn chỉnh được xây dựng bằng ASP.NET MVC với thiết kế hiện đại và đầy đủ chức năng quản lý khóa học.

## ✨ Tính Năng

### 🔐 Chức Năng Chung (General Features)

| ID | Chức Năng | Mô Tả |
|---|---|---|
| **G.01** | Đăng ký/Đăng nhập | Tạo tài khoản mới và đăng nhập bằng email/mật khẩu |
| **G.02** | Quản lý Hồ sơ | Xem và cập nhật thông tin cá nhân (tên, mật khẩu, ảnh đại diện) |
| **G.03** | Tìm kiếm | Tìm kiếm khóa học theo tên, danh mục, giảng viên |
| **G.04** | Thanh toán | Tích hợp sẵn sàng cho Stripe, PayPal, VNPay |

### 👨‍💼 Chức Năng Admin

#### Quản lý Khóa học (Course Management)

| ID | Chức Năng | Chi tiết |
|---|---|---|
| **ADM.01** | Thêm Khóa học (CRUD) | Tạo khóa học mới với đầy đủ thông tin |
| **ADM.02** | Sửa/Xóa Khóa học | Cập nhật hoặc xóa khóa học |
| **ADM.03** | Quản lý Bài học | Thêm, sửa, xóa bài học (video, tài liệu) |
| **ADM.04** | Quản lý Danh mục | Tạo, sửa, xóa danh mục khóa học |

#### Quản lý Người dùng (User Management)

| ID | Chức Năng | Chi tiết |
|---|---|---|
| **ADM.05** | Xem danh sách User | Xem tất cả tài khoản User và Admin |
| **ADM.06** | Cập nhật Vai trò | Thay đổi vai trò User/Admin |
| **ADM.07** | Vô hiệu hóa User | Khóa/Mở khóa tài khoản |

#### Quản lý Giao dịch & Báo cáo

| ID | Chức Năng | Chi tiết |
|---|---|---|
| **ADM.08** | Xem Đơn hàng | Xem lịch sử mua khóa học |
| **ADM.09** | Báo cáo cơ bản | Thống kê User, khóa học, doanh thu |

### 👨‍🎓 Chức Năng User

#### Trải nghiệm Khóa học

| ID | Chức Năng | Chi tiết |
|---|---|---|
| **USR.01** | Xem chi tiết Khóa học | Xem giới thiệu, mục lục, đánh giá |
| **USR.02** | Mua Khóa học | Thanh toán để sở hữu khóa học |
| **USR.03** | Thư viện Khóa học | Danh sách khóa học đã mua |
| **USR.04** | Xem Bài học (Player) | Truy cập video/tài liệu |
| **USR.05** | Theo dõi Tiến độ | Lưu tiến độ học tập tự động |

#### Tương tác

| ID | Chức Năng | Chi tiết |
|---|---|---|
| **USR.06** | Đánh giá & Bình luận | Để lại đánh giá (1-5 sao) và bình luận |
| **USR.07** | Lịch sử Giao dịch | Xem lại các đơn hàng |

## 🛠️ Công Nghệ Sử Dụng

- **Framework**: ASP.NET Core 9.0 MVC
- **Database**: SQL Server với Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Custom CSS với Dark Theme
- **Icons**: Font Awesome 6.4.0

## 📦 Cài Đặt

### Yêu Cầu

- .NET 9.0 SDK
- SQL Server hoặc SQL Server LocalDB
- Visual Studio 2022 hoặc VS Code

### Các Bước Cài Đặt

1. **Clone repository**
```bash
git clone <repository-url>
cd nguyenthanhlapNew
```

2. **Cài đặt dependencies**
```bash
dotnet restore
```

3. **Cập nhật connection string**

Mở `appsettings.json` và cập nhật connection string nếu cần:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=nguyenthanhlapDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. **Tạo database**
```bash
dotnet ef database update
```

5. **Chạy ứng dụng**
```bash
dotnet run
```

6. **Truy cập ứng dụng**

Mở trình duyệt và truy cập: `https://localhost:5001`

## 👤 Tài Khoản Mặc Định

### Admin
- **Email**: admin@khoahoc.com
- **Password**: Admin@123

## 📁 Cấu Trúc Dự Án

```
nguyenthanhlapNew/
├── Areas/
│   └── Admin/
│       └── Controllers/      # Admin controllers
├── Controllers/              # User controllers
├── Data/                     # DbContext
├── Models/                   # Entity models
├── ViewModels/              # View models
├── Views/                   # Razor views
│   ├── Account/            # Authentication views
│   ├── Courses/            # Course views
│   ├── Home/               # Home views
│   ├── Payment/            # Payment views
│   └── Shared/             # Shared layouts
├── wwwroot/                # Static files
│   ├── css/               # Stylesheets
│   ├── js/                # JavaScript
│   └── uploads/           # User uploads
└── Migrations/            # EF migrations
```

## 🎨 Thiết Kế

Website sử dụng thiết kế hiện đại với:
- **Dark Theme**: Giao diện tối chuyên nghiệp
- **Gradient Colors**: Màu sắc gradient đẹp mắt
- **Smooth Animations**: Hiệu ứng chuyển động mượt mà
- **Responsive Design**: Tương thích mọi thiết bị
- **Modern Typography**: Font chữ Inter cao cấp

## 🔧 Cấu Hình

### Tích Hợp Payment Gateway

Để tích hợp cổng thanh toán thực tế, cập nhật `PaymentController.cs`:

#### Stripe
```csharp
// Thêm Stripe package
dotnet add package Stripe.net

// Cấu hình trong appsettings.json
"Stripe": {
  "SecretKey": "your-secret-key",
  "PublishableKey": "your-publishable-key"
}
```

#### VNPay
```csharp
// Cấu hình trong appsettings.json
"VNPay": {
  "TmnCode": "your-tmn-code",
  "HashSecret": "your-hash-secret",
  "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
}
```

## 📝 Database Schema

### Tables

- **AspNetUsers**: Người dùng (kế thừa Identity)
- **Categories**: Danh mục khóa học
- **Courses**: Khóa học
- **Lessons**: Bài học
- **Orders**: Đơn hàng
- **CourseProgresses**: Tiến độ học tập
- **Reviews**: Đánh giá khóa học

## 🚀 Tính Năng Nâng Cao

### Upload Files

Hệ thống hỗ trợ upload:
- Ảnh bìa khóa học
- Ảnh đại diện người dùng
- Video bài học
- Tài liệu PDF

### Video Player

Hỗ trợ:
- YouTube embed
- Vimeo embed
- HTML5 video player
- Tua, tạm dừng, điều chỉnh tốc độ

### Progress Tracking

- Tự động lưu bài học cuối cùng
- Hiển thị % hoàn thành
- Đánh dấu bài học đã xem

## 🔒 Bảo Mật

- Password hashing với Identity
- CSRF protection
- XSS prevention
- SQL injection protection (EF Core)
- Role-based authorization

## 📱 Responsive Design

Website hoạt động tốt trên:
- Desktop (1920px+)
- Laptop (1366px+)
- Tablet (768px+)
- Mobile (320px+)

## 🐛 Troubleshooting

### Lỗi Database Connection

Nếu gặp lỗi kết nối database:
1. Kiểm tra SQL Server đã chạy
2. Xác nhận connection string đúng
3. Chạy lại migration: `dotnet ef database update`

### Lỗi Build

Nếu gặp lỗi build:
```bash
dotnet clean
dotnet restore
dotnet build
```

## 📄 License

This project is licensed under the MIT License.

## 👥 Contributors

- Developer: AI Assistant
- Framework: ASP.NET Core Team

## 📞 Support

Nếu cần hỗ trợ, vui lòng tạo issue trên GitHub repository.

---

**Chúc bạn học tập vui vẻ! 🎓**

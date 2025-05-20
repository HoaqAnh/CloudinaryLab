# 🚀 Cloudinary File Management Lab (ASP.NET Core & React)

---

## Giới thiệu

Đây là một dự án lab toàn diện được xây dựng để thử nghiệm và minh họa khả năng **upload, hiển thị, cập nhật và xóa (CRUD) file** sử dụng **Cloudinary** làm dịch vụ lưu trữ đám mây. Backend được phát triển bằng **ASP.NET Core API**, trong khi frontend sử dụng **React, TypeScript và Vite**, mang đến trải nghiệm người dùng tương tác để quản lý hình ảnh.

Dự án này là một tài nguyên tuyệt vời cho những ai muốn tìm hiểu về cách tích hợp Cloudinary vào ứng dụng web của mình với công nghệ .NET và React hiện đại.

## Tính năng nổi bật

* **Upload ảnh:** Dễ dàng chọn và upload nhiều ảnh lên Cloudinary.
* **Hiển thị ảnh:** Xem tất cả các ảnh đã upload.
* **Xóa ảnh:** Xóa ảnh vĩnh viễn khỏi Cloudinary thông qua giao diện người dùng.
* **API mạnh mẽ:** Backend RESTful API được xây dựng với ASP.NET Core.
* **Giao diện người dùng hiện đại:** Frontend được phát triển với React, TypeScript và Vite, mang lại trải nghiệm người dùng mượt mà và trực quan.

## Công nghệ sử dụng

### Backend
* **ASP.NET Core:** Framework mạnh mẽ để xây dựng API.
* **CloudinaryDotNet:** Thư viện .NET chính thức để tương tác với Cloudinary.

### Frontend
* **React:** Thư viện JavaScript phổ biến để xây dựng giao diện người dùng.
* **TypeScript:** Ngôn ngữ giúp tăng cường khả năng bảo trì và giảm lỗi cho mã nguồn JavaScript.
* **Vite:** Công cụ build nhanh và nhẹ cho các dự án frontend.
* **Axios:** Client HTTP dựa trên Promise để tạo các yêu cầu tới API.

### Dịch vụ đám mây
* **Cloudinary:** Nền tảng quản lý tệp hình ảnh và video trên đám mây.

## Điều kiện tiên quyết

Để chạy dự án này trên máy cục bộ của bạn, bạn cần cài đặt các công cụ sau:

* **[.NET SDK](https://dotnet.microsoft.com/download)** (Phiên bản 8.0 trở lên được khuyến nghị)
* **[Node.js](https://nodejs.org/)** (Phiên bản LTS được khuyến nghị) và **npm** hoặc **yarn**
* **[Download Git](https://git-scm.com/downloads)**
* Một tài khoản **[Cloudinary](https://cloudinary.com/)** (Gói miễn phí là đủ cho mục đích thử nghiệm)

## Hướng dẫn cài đặt và chạy

Thực hiện theo các bước dưới đây để thiết lập và chạy dự án trên máy cục bộ của bạn.

### 1. Clone repository
```
git clone https://github.com/HoaqAnh/CloudinaryLab.git
```
### 2. Cấu hình Cloudinary

Truy cập Dashboard Cloudinary của bạn để lấy Cloud Name, API Key và API Secret.
- Cấu hình Backend (ASP.NET Core):
  - Di chuyển vào thư mục backend của dự án
  - Tạo file `appsettings.json` (nếu chưa có) trong thư mục Cloud và thêm thông tin cấu hình Cloudinary của bạn:
     ```
      {
        "CloudinarySettings": {
          "CloudName": "YOUR_CLOUD_NAME",
          "ApiKey": "YOUR_API_KEY",
          "ApiSecret": "YOUR_API_SECRET"
        },
        "Logging": {
          "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
          }
        },
        "AllowedHosts": "*"
      }
    ```
- Cấu hình Frontend (React):
  - Di chuyển vào thư mục frontend của dự án
  - Tạo một file `.env` trong thư mục gốc của dự án frontend (nếu chưa có) và thêm đường dẫn API của backend:
    ```
    VITE_API_BASE_URL=http://localhost:5077/api # Hoặc port mà API của bạn đang chạy
    ```
### 3. Chạy Backend (ASP.NET Core API)

Trong thư mục backend `(back-end/)`, chạy các lệnh sau:
```
dotnet restore
dotnet build
dotnet run
```
API sẽ khởi chạy, trên terminal sẽ hiển thị port (thường là trên http://localhost:5000 hoặc http://localhost:5001 (HTTPS)).

### 4. Chạy Frontend (React)

Trong thư mục frontend `(front-end/)`, chạy các lệnh sau:
```
npm install # Hoặc yarn install để cài đặt các dependency
npm run dev # Hoặc yarn dev để khởi chạy ứng dụng
```
Ứng dụng frontend sẽ được mở trong trình duyệt của bạn, thường là tại `http://localhost:5173`

## Cách sử dụng

Sau khi cả backend và frontend đã được chạy thành công, bạn có thể tương tác với ứng dụng thông qua trình duyệt:
- Tải ảnh lên: Sử dụng form tải lên để chọn và gửi ảnh đến Cloudinary.
- Xem thư viện: Duyệt qua gallery ảnh để xem tất cả các ảnh bạn đã tải lên.
- Quản lý ảnh: Tương tác với các nút hoặc hành động để xem hoặc xóa các ảnh cụ thể.

## Thành viên nhóm

| Họ và tên           | Vai trò                   | GitHub                                          |
| ------------------- | ------------------------  | ------------------------------------------------|
| Trần Phạm Hoàng Anh | Frontend & UI/UX Designer | [@HoaqAnh](https://github.com/HoaqAnh)          |
| Nguyễn Toàn Thắng   | Backend & Cloudinary Dev  | [@imthq1](https://github.com/imthq1)          |
| Nguyễn Đức Thiện    | Backend Developer         | [@nguyenducthienlq1](https://github.com/nguyenducthienlq1) |
| Lê Hoàng Gia Đại    | Project Management        | [@PeterHovng](https://github.com/PeterHovng)    |

## Đóng góp

Mọi đóng góp đều được chào đón! Nếu bạn có bất kỳ đề xuất nào, phát hiện lỗi hoặc muốn thêm tính năng mới, vui lòng mở một issue hoặc gửi một pull request.

Hướng dẫn cài đặt và khởi chạy RabbitMQ bằng Docker
Bước 1: Cài đặt Docker
Trước tiên, bạn cần cài đặt Docker trên máy tính của mình. Bạn có thể tải Docker từ trang chính thức của Docker: https://www.docker.com/products/docker-desktop

Bước 2: Khởi chạy RabbitMQ bằng Docker
Mở PowerShell và chạy lệnh sau để khởi chạy server RabbitMQ trên Docker:

powershell
Copy code
docker run -d --name my-rabbit -p 8080:15672 -p 5673:5672 rabbitmq:3-management
Lệnh trên sẽ tải và chạy container RabbitMQ phiên bản 3 có tích hợp giao diện quản lý trên cổng 8080. Bạn có thể truy cập giao diện quản lý của RabbitMQ tại http://localhost:8080/

Để đăng nhập, sử dụng tên người dùng (UserName) là "guest" và mật khẩu (Password) cũng là "guest".

Bước 3: Import file Database
TutionDB
File TutionDB được sử dụng cho dịch vụ Tution (tution-service). Bạn có thể import file này vào cơ sở dữ liệu của mình để sử dụng dịch vụ Tution.

BankDB
File BankDB được sử dụng cho dịch vụ Authentication (authentication-service) và Banking (banking-service). Bạn cũng cần import file này vào cơ sở dữ liệu của mình để sử dụng các dịch vụ liên quan đến xác thực và ngân hàng.

Với các bước trên, bạn đã cài đặt và khởi chạy RabbitMQ bằng Docker thành công và sẵn sàng sử dụng cho dự án của mình.

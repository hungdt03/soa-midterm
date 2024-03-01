# Hướng dẫn cài đặt và sử dụng RabbitMQ và import cơ sở dữ liệu
### 1. Cài đặt RabbitMQ bằng Docker
Mở Powershell và chạy lệnh sau để start server RabbitMQ trên Docker:
```
docker run -d --name my-rabbit -p 8080:15672 -p 5673:5672 rabbitmq:3-management
```
Truy cập vào RabbitMQ management console:\
Trình duyệt web: http://localhost:8080/
#### Đăng nhập với tên người dùng và mật khẩu mặc định:
Username: guest\
Password: guest
### 2. Import cơ sở dữ liệu
Thư mục SQL\
Trong thư mục Sql, bạn sẽ tìm thấy 2 tệp cơ sở dữ liệu:\
TutionDB.sql: Dùng cho dịch vụ tution-service.\
BankDB.sql: Dùng cho dịch vụ authentication-service và banking-service.



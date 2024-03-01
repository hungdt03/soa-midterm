Mở powershell và chạy lệnh để start server RabbitMQ trên docker: docker run -d --name my-rabbit -p 8080:15672 -p 5673:5672 rabbitmq:3-management
Ta sẽ đã cài đặt thành công RabbitMQ bằng docker chạy trên: http://localhost:8080/
Để login ta sử dụng UserName: guest và Password: guest mặc định được tạo ra.

Import 2 file Database trong thư mục Sql
TutionDB dùng cho tution-service
BankDB dùng cho authentication-service và banking-service

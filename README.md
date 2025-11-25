#  Hệ Thống Quản Lý Bệnh Viện

##  Giới Thiệu

Dự án **Hệ Thống Quản Lý Bệnh Viện** là một ứng dụng được thiết kế để số hóa và tối ưu hóa các quy trình quản lý trong môi trường y tế. Hệ thống cung cấp các công cụ để quản lý thông tin bệnh nhân, hồ sơ bệnh án, lịch hẹn khám, và các hoạt động khác liên quan đến vận hành bệnh viện.

### Mục Tiêu Dự Án

- Số hóa quy trình quản lý hồ sơ bệnh án
- Tối ưu hóa quy trình làm việc cho nhân viên y tế
- Cải thiện trải nghiệm của bệnh nhân
- Đảm bảo tính bảo mật và toàn vẹn dữ liệu y tế

##  Tính Năng

###  Quản Lý Người Dùng
- Đăng nhập/Đăng xuất với xác thực bảo mật
- Phân quyền theo vai trò (Admin, Bác sĩ, Y tá, Lễ tân)
- Quản lý hồ sơ người dùng

###  Quản Lý Bệnh Nhân
- **Thêm** bệnh nhân mới với thông tin chi tiết
- **Xem** danh sách và hồ sơ bệnh nhân
- **Cập nhật** thông tin bệnh nhân
- **Xóa** hồ sơ bệnh nhân (với phân quyền phù hợp)
- Tìm kiếm và lọc bệnh nhân theo nhiều tiêu chí

###  Quản Lý Hồ Sơ Bệnh Án
- Tạo và cập nhật hồ sơ bệnh án
- Ghi nhận triệu chứng, chẩn đoán, và phác đồ điều trị
- Lưu trữ lịch sử khám bệnh
- Quản lý đơn thuốc và kết quả xét nghiệm

###  Quản Lý Lịch Hẹn
- Đặt lịch hẹn khám cho bệnh nhân
- Xem lịch làm việc của bác sĩ
- Thông báo lịch hẹn tự động
- Quản lý trạng thái lịch hẹn

###  Quản Lý Thuốc
- Danh mục thuốc và vật tư y tế
- Quản lý kho thuốc
- Kê đơn thuốc điện tử
- Theo dõi tồn kho

###  Báo Cáo và Thống Kê
- Thống kê số lượng bệnh nhân
- Báo cáo doanh thu
- Phân tích hiệu suất hoạt động
- Xuất báo cáo dưới nhiều định dạng

##  Công Nghệ Sử Dụng

- **Ngôn ngữ lập trình**: C#
- **Framework**: XAML (cho giao diện desktop)
- **Cơ sở dữ liệu**: MySQL / PostgreSQL
- **Build tool**: Maven / Gradle
- **IDE**: Eclipse / IntelliJ IDEA / NetBeans

##  Yêu Cầu Hệ Thống

### Phần Cứng
- **CPU**: Intel Core i3 hoặc tương đương
- **RAM**: Tối thiểu 4GB
- **Ổ cứng**: 500MB dung lượng trống

### Phần Mềm
- **JDK**: Phiên bản 11 trở lên
- **Database**: MySQL 5.7+ hoặc PostgreSQL 10+
- **Hệ điều hành**: Windows 10/11, macOS, hoặc Linux

##  Cài Đặt

### 1. Clone Repository

```bash
git clone https://github.com/ThanhNguyen301004/quanlybenhvien.git
cd quanlybenhvien
```

### 2. Cấu Hình Database

Tạo database mới:

```sql
CREATE DATABASE hospital_management;
```

Import file SQL schema:

```bash
mysql -u username -p hospital_management < database/schema.sql
```

### 3. Cấu Hình Kết Nối

Chỉnh sửa file `config/database.properties`:

```properties
db.host=localhost
db.port=3306
db.name=hospital_management
db.username=your_username
db.password=your_password
```

### 4. Build và Chạy Ứng Dụng

Sử dụng Maven:

```bash
mvn clean install
mvn exec:java
```

Hoặc Gradle:

```bash
gradle build
gradle run
```

##  Sử Dụng

### Đăng Nhập Lần Đầu

Sử dụng tài khoản mặc định:

- **Username**: `admin`
- **Password**: `admin123`

 *Khuyến nghị thay đổi mật khẩu sau lần đăng nhập đầu tiên.*

### Quy Trình Sử Dụng Cơ Bản

1. **Đăng nhập** vào hệ thống với tài khoản được cấp
2. **Thêm bệnh nhân mới** từ menu "Quản lý bệnh nhân"
3. **Tạo hồ sơ bệnh án** cho bệnh nhân
4. **Đặt lịch hẹn** khám bệnh
5. **Ghi nhận kết quả** khám và điều trị
6. **Xuất báo cáo** khi cần thiết

#



##  Nhóm Phát Triển

**Nhóm 1 - Đồ Án Nhóm**

- **Nguyễn Văn Thành** 
- **Lê Thuận An**
- **Lê Đình Hoài Bảo**
- **Cái Xuân Hòa**
- **Lê Duy Khánh**
- **Trần Thanh Sỹ**
- **Lê Vĩnh Toàn**
- **Tô Trần Tuyển**


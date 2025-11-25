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
- **Cơ sở dữ liệu**: ADO.NET
- **IDE**: Visual Studio


## 📦 Cài Đặt

### Bước 1: Clone Repository

```bash
git clone https://github.com/ThanhNguyen301004/quanlybenhvien.git
cd quanlybenhvien
```

### Bước 2: Cài Đặt SQL Server

1. Tải và cài đặt **SQL Server** (nếu chưa có):
   - Download từ: https://www.microsoft.com/sql-server/sql-server-downloads
   - Chọn phiên bản Express (miễn phí) hoặc Developer Edition

2. Cài đặt **SQL Server Management Studio (SSMS)**:
   - Download từ: https://aka.ms/ssmsfullsetup

### Bước 3: Tạo Database

1. Mở **SQL Server Management Studio (SSMS)**
2. Kết nối đến SQL Server instance của bạn
3. Tạo database mới:

```sql
CREATE DATABASE HospitalManagement;
GO
```

4. Restore database từ backup (nếu có file .bak):

```sql
USE master;
GO
RESTORE DATABASE HospitalManagement
FROM DISK = N'D:\path\to\HospitalManagement.bak'
WITH REPLACE;
GO
```

**HOẶC** chạy script tạo bảng (nếu có file .sql):

```sql
USE HospitalManagement;
GO
-- Chạy nội dung file database/schema.sql
```

### Bước 4: Cấu Hình Connection String

1. Mở solution trong Visual Studio
2. Tìm file `App.config` hoặc `Web.config`
3. Cập nhật connection string:

```xml
<connectionStrings>
  <add name="HospitalEntities" 
       connectionString="metadata=res://*/Models.HospitalModel.csdl|res://*/Models.HospitalModel.ssdl|res://*/Models.HospitalModel.msl;
                        provider=System.Data.SqlClient;
                        provider connection string=&quot;
                        data source=YOUR_SERVER_NAME;
                        initial catalog=HospitalManagement;
                        integrated security=True;
                        MultipleActiveResultSets=True;
                        App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

**Lưu ý**: 
- Thay `YOUR_SERVER_NAME` bằng tên SQL Server của bạn (vd: `localhost`, `.\SQLEXPRESS`, hoặc `(LocalDB)\MSSQLLocalDB`)
- Nếu dùng SQL Authentication, thêm: `User ID=sa;Password=your_password;Integrated Security=False`

### Bước 5: Restore NuGet Packages

1. Trong Visual Studio, mở **Tools** > **NuGet Package Manager** > **Package Manager Console**
2. Chạy lệnh:

```powershell
Update-Package -reinstall
```

Hoặc click chuột phải vào Solution > **Restore NuGet Packages**

### Bước 6: Update Entity Data Model (nếu cần)

1. Mở file `.edmx` trong Solution Explorer
2. Click chuột phải vào designer surface > **Update Model from Database**
3. Chọn các bảng, views, stored procedures cần thêm/cập nhật
4. Click **Finish**

### Bước 7: Build và Run

1. Chọn **Build** > **Build Solution** (hoặc Ctrl + Shift + B)
2. Đảm bảo không có lỗi compile
3. Nhấn **F5** hoặc click **Start** để chạy ứng dụng

### Khắc Phục Sự Cố

**Lỗi kết nối database:**
- Kiểm tra SQL Server có đang chạy không (SQL Server Configuration Manager)
- Đảm bảo TCP/IP được enable trong SQL Server Configuration
- Kiểm tra firewall không block port 1433

**Lỗi Entity Framework:**
- Đảm bảo đã cài đặt đúng package: `EntityFramework` (version 6.x)
- Kiểm tra connection string có đúng format không

**Lỗi thiếu package:**
```powershell
Install-Package EntityFramework -Version 6.4.4
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

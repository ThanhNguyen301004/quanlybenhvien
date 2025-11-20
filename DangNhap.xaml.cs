using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QUANLYBENHVIEN
{
    public partial class DangNhap : Window
    {
        QLBVEntities db = new QLBVEntities();

        public DangNhap()
        {
            InitializeComponent();
        }

        private void Button_Click_DangKy(object sender, RoutedEventArgs e)
        {
            new DangKy().Show();
            this.Close();
        }

        private void Button_Click_DangNhap(object sender, RoutedEventArgs e)
        {
            string tk = Tb_taiKhoan.Text.Trim();
            string mk = Tb_matKhau.Password.Trim();
            string loaiTkChon = (Cbb_loaiTaiKhoan.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(tk) || string.IsNullOrEmpty(mk) || string.IsNullOrEmpty(loaiTkChon))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy user từ DB
            var user = db.TAIKHOANs
                          .FirstOrDefault(x => x.TaiKhoan1 == tk && x.MaKhau == mk);

            if (user == null)
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Phân quyền đăng nhập
            if (user.LoaiTK != loaiTkChon)
            {
                MessageBox.Show("Bạn không có quyền đăng nhập với vai trò này!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mở form tương ứng
            Window nextWindow = user.LoaiTK switch
            {
                "Quản trị viên" => new MainWindow(user.TaiKhoan1),
                "Khách" => new KhachHang(user.TaiKhoan1),
                _ => null
            };

            if (nextWindow != null)
            {
                nextWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Loại tài khoản không hợp lệ!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

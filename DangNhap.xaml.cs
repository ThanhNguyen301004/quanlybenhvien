using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            DangKy dk = new DangKy();
            dk.Show();
            this.Close();
        }

        private void Button_Click_DangNhap(object sender, RoutedEventArgs e)
        {
            string tk = Tb_taiKhoan.Text.Trim();
            string mk = Tb_matKhau.Password.Trim();

            string loaiTkChon = (Cbb_loaiTaiKhoan.SelectedItem as ComboBoxItem)?.Content.ToString();

            using (var db = new QLBVEntities())
            {
                var user = db.TAIKHOANs.FirstOrDefault(x => x.TaiKhoan1 == tk && x.MaKhau == mk);

                if (user != null)
                {
                    if (loaiTkChon == "Quản trị viên" && user.LoaiTK == "Quản trị viên")
                    {
                        MainWindow main = new MainWindow(user.TaiKhoan1);
                        main.Show();
                        this.Close();
                    }
                    else if (loaiTkChon == "Khách" && user.LoaiTK == "Khách")
                    {
                        KhachHang kh = new KhachHang(user.TaiKhoan1); // bạn tạo thêm form KhachWindow
                        kh.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Bạn không có quyền đăng nhập với vai trò này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}

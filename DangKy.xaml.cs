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
    /// <summary>
    /// Interaction logic for DangKy.xaml
    /// </summary>
    public partial class DangKy : Window
    {
        public DangKy()
        {
            InitializeComponent();
        }

        private void Button_Click_DangNhap(object sender, RoutedEventArgs e)
        {
            DangNhap dk = new DangNhap();
            dk.Show();
            this.Close();
        }

        private void Button_Click_DangKy(object sender, RoutedEventArgs e)
        {
            string taiKhoan = Tb_taiKhoan.Text.Trim();
            string matKhau = Tb_matKhau.Password.Trim();
            string nhapLai = Tb_nhapLaiMatKhau.Password.Trim();

            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }
            if (matKhau != nhapLai)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!");
                return;
            }

            using (var db = new QLBVEntities())
            {
                // kiểm tra trùng tài khoản
                if (db.TAIKHOANs.Any(x => x.TaiKhoan1 == taiKhoan))
                {
                    MessageBox.Show("Tài khoản đã tồn tại!");
                    return;
                }

                // thêm mới
                TAIKHOAN tk = new TAIKHOAN
                {
                    TaiKhoan1 = taiKhoan,
                    MaKhau = matKhau,
                    LoaiTK = "Khách", // luôn gán là khách
                    AnhCaNhan = null
                };

                db.TAIKHOANs.Add(tk);
                db.SaveChanges();
            }

            MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.");

            DangNhap dn = new DangNhap();
            dn.Show();
            this.Close();
        }


    }
}

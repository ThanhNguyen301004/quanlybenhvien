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
    /// Interaction logic for KhachHang.xaml
    /// </summary>
    public partial class KhachHang : Window
    {
        public KhachHang(string TaiKhoan)
        {
            InitializeComponent();
            MainFrame.Navigate(new TrangChuKhach());
            txtUserName.Text = $"Xin chào {TaiKhoan} ";
        }

        private User_DanhSachBacSi danhSachBacSiPage;

        private void Button_Click_BacSi(object sender, RoutedEventArgs e)
        {
            if (danhSachBacSiPage == null)
                danhSachBacSiPage = new User_DanhSachBacSi();

            MainFrame.Navigate(danhSachBacSiPage);
        }

        private void Button_Click_TrangChu(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TrangChuKhach());
        }

        private void Button_Click_DuDoanBenhTieuDuong(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DuDoanBenhTieuDuong());
        }


    }
}

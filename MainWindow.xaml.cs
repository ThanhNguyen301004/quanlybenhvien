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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string TaiKhoan)
        {
            InitializeComponent();
            frameContent.Navigate(new TrangChu());
            txtUserName.Text = TaiKhoan;
        }

        private void MenuItem_Click_QuanLyBenhNhan(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyBenhNhan());
        }

        private void MenuItem_Click_QuanLyBacSi(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyBacSi());
        }

        private void MenuItem_Click_QuanLyLichHen(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyLichHen());
        }

        private void MenuItem_Click_QuanLyPhongBenh(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyPhongBenh());
        }

        private void MenuItem_Click_QuanLyHoaDonVienPhi(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyHoaDonVienPhi());
        }

        private void MenuItem_Click_ThongKeDoanhThu(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new ThongKeDoanhThu());
        }

        private void MenuItem_Click_LichSuHoaDon(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new LichSuHoaDon());
        }

        private void MenuItem_Click_InHoaDon(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new InHoaDon());
        }


        private void MenuItem_Click_Dashboard(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new TrangChu());
        }

        private void MenuItem_Click_Logout(object sender, RoutedEventArgs e)
        {
            DangNhap dn = new DangNhap();
            dn.Show();
            this.Close();
        }

        private void MenuItem_Click_QuanLyTaiKhoan(object sender, RoutedEventArgs e)
        {
            frameContent.Navigate(new QuanLyTaiKhoan());

        }
    }
}

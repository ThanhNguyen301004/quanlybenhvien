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
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class TrangChu : Page
    {
        QLBVEntities db = new QLBVEntities();
        // Lấy 5 lịch hẹn gần nhất
        

        public TrangChu()
        {
            InitializeComponent();

            Tb_tongBenhNhan.Text = db.BENHNHANs.Count().ToString();
            
            Tb_tongBacSi.Text = db.BACSIs.Count().ToString();
            
            DateTime today = DateTime.Today;
            Tb_lichHenHomNay.Text = db.LICHHENs
                                      .Where(lh => lh.NgayHen == today)
                                      .Count()
                                      .ToString();
            
            int tongPhong = db.PHONGBENHs.Count();
            int phongDangSuDung = db.PHONGBENHs
                                    .Count(p => p.TrangThai == "Đang sử dụng");

            if (tongPhong > 0)
            {
                double tiLe = (double)phongDangSuDung / tongPhong * 100;
                Tb_tyLeSuDungPhong.Text = tiLe.ToString("0.##") + "%";
            }
            else
            {
                Tb_tyLeSuDungPhong.Text = "0%";
            }
            DateTime firstDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            decimal doanhThuThang = db.HOADONVIENPHIs
                .Where(hd => hd.NgayNhapVien >= firstDay && hd.NgayNhapVien <= lastDay)
                .Sum(hd => (decimal?)hd.ThanhTien) ?? 0;

            Tb_doanhThuThang.Text = doanhThuThang.ToString("N0") + " VNĐ";


            var thongKeKhoa = db.BACSIs
                .GroupBy(bs => bs.ChuyenKhoa)
                .Select(g => new
                {
                    Khoa = g.Key,
                    SoLuong = g.Count()
                })
                .ToList();
            
        }

        private void Button_Click_BenhNhan(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuanLyBenhNhan());
        }


        private void Button_Click_LichHen(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuanLyLichHen());
        }

        private void Button_Click_PhongBenh(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuanLyPhongBenh());
        }

        private void Button_Click_BacSi(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuanLyBacSi());
        }

        private void Button_Click_HoaDon(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new QuanLyHoaDonVienPhi());
        }
    }
}

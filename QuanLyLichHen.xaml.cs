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
using System.Data.Entity;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for QuanLyLichHen.xaml
    /// </summary>
    public partial class QuanLyLichHen : Page
    {
        QLBVEntities db = new QLBVEntities();

        public QuanLyLichHen()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var newdb = new QLBVEntities())
            {
                DG_LichHen.ItemsSource = newdb.LICHHENs
                    .Include("BENHNHAN")
                    .Include("BACSI")
                    .ToList();
            }
        }

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            Form_LichHen flh = new Form_LichHen();
            bool? result = flh.ShowDialog();
            if (result == true)
            {
                LoadData();
            }
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maLH))
            {
                using (var newDb = new QLBVEntities())
                {
                    var lh = newDb.LICHHENs.Find(maLH);
                    if (lh != null)
                    {
                        Form_LichHen flh = new Form_LichHen(lh);
                        bool? result = flh.ShowDialog();
                        if (result == true)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }


        private void Btn_Xoa_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maLH))
            {
                var lh = db.LICHHENs.Find(maLH);
                if (lh != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa lịch hẹn: {lh.MaLichHen}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.LICHHENs.Remove(lh);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            Exception ie = ex;
                            while (ie.InnerException != null) ie = ie.InnerException;

                            MessageBox.Show("Không thể xóa lịch hẹn này. Chi tiết: " + ie.Message,
                                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Tb_timKiem.Text == "Tìm kiếm...")
            {
                Tb_timKiem.Text = "";
                Tb_timKiem.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Tb_timKiem.Text))
            {
                Tb_timKiem.Text = "Tìm kiếm...";
                Tb_timKiem.Foreground = Brushes.Gray;
            }
        }

        private void Button_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string keyword = Tb_timKiem.Text.Trim();

            if (keyword == "Tìm kiếm...")
                keyword = "";

            if (string.IsNullOrEmpty(keyword))
            {
                LoadData();
                return;
            }

            using (var newdb = new QLBVEntities())
            {
                var query = newdb.LICHHENs.AsQueryable();

                switch (ComboBox_TimKiem.SelectedIndex)
                {
                    case 0: // Bệnh nhân
                        query = query.Where(lh => lh.BENHNHAN.HoTen.Contains(keyword));
                        break;
                    case 1: // Bác sĩ
                        query = query.Where(lh => lh.BACSI.HoTen.Contains(keyword));
                        break;
                    case 2: // Ngày hẹn
                        if (DateTime.TryParse(keyword, out DateTime ngay))
                        {
                            query = query.Where(lh => lh.NgayHen.Date == ngay.Date);
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng nhập đúng định dạng ngày (vd: 22/09/2025)");
                            return;
                        }
                        break;
                    case 3: // Mục đích
                        query = query.Where(lh => lh.MucDich.Contains(keyword));
                        break;
                    case 4: // Trạng thái
                        query = query.Where(lh => lh.TrangThai.Contains(keyword));
                        break;
                    default:
                        break;
                }

                DG_LichHen.ItemsSource = query
                    .Include("BENHNHAN")
                    .Include("BACSI")
                    .ToList();
            }
        }

    }

}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data.Entity;

//
namespace QUANLYBENHVIEN
{
    public partial class QuanLyHoaDonVienPhi : Page
    {
        QLBVEntities db = new QLBVEntities();

        public QuanLyHoaDonVienPhi()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var newdb = new QLBVEntities())
            {
                DG_HoaDon.ItemsSource = newdb.HOADONVIENPHIs
                    .Include("BENHNHAN")
                    .Include("PHONGBENH")
                    .ToList();
            }
        }


        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            // Mở form nhập hóa đơn
            Form_HoaDonVienPhi fhd = new Form_HoaDonVienPhi();
            bool? result = fhd.ShowDialog();
            if (result == true)
            {
                LoadData();
            }
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maHD))
            {
                using (var newDb = new QLBVEntities())
                {
                    var hd = newDb.HOADONVIENPHIs.Find(maHD);
                    if (hd != null)
                    {
                        Form_HoaDonVienPhi fbn = new Form_HoaDonVienPhi(hd);
                        bool? result = fbn.ShowDialog();
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
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maHD))
            {
                var hd = db.HOADONVIENPHIs.Find(maHD);
                if (hd != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa hóa đơn: {hd.MaHoaDon}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.HOADONVIENPHIs.Remove(hd);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            Exception ie = ex;
                            while (ie.InnerException != null) ie = ie.InnerException;

                            MessageBox.Show("Không thể xóa hóa đơn này. Chi tiết: " + ie.Message,
                                            "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void Button_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string tuKhoa = Tb_timKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                LoadData(); // hiện tất cả dữ liệu
                return;
            }

            using (var db = new QLBVEntities())
            {
                var query = db.HOADONVIENPHIs.AsQueryable();

                switch (ComboBox_TimKiem.SelectedIndex)
                {
                    case 0: // Bệnh nhân
                        query = query.Where(hd => hd.BENHNHAN.HoTen.Contains(tuKhoa));
                        break;

                    case 1: // Phòng bệnh
                        query = query.Where(hd => hd.PHONGBENH.SoPhong.Contains(tuKhoa));
                        break;

                    case 2: // Ngày nhập viện
                        if (DateTime.TryParse(tuKhoa, out DateTime ngayNhap))
                        {
                            query = query.Where(hd => hd.NgayNhapVien == ngayNhap);
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng nhập đúng định dạng ngày (dd/MM/yyyy).");
                            return;
                        }
                        break;

                    case 3: // Ngày xuất viện
                        if (DateTime.TryParse(tuKhoa, out DateTime ngayXuat))
                        {
                            query = query.Where(hd => hd.NgayXuatVien == ngayXuat);
                        }
                        else
                        {
                            MessageBox.Show("Vui lòng nhập đúng định dạng ngày (dd/MM/yyyy).");
                            return;
                        }
                        break;
                }

                DG_HoaDon.ItemsSource = query
                    .Include("BENHNHAN")
                    .Include("PHONGBENH")
                    .ToList();
               
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
    }
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QUANLYBENHVIEN
{
    public partial class QuanLyPhongBenh : Page
    {
        private readonly QLBVEntities db = new QLBVEntities();

        public QuanLyPhongBenh()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var data = db.PHONGBENHs
                .Select(pb => new PhongBenhViewModel
                {
                    MaPhong = pb.MaPhong,
                    SoPhong = pb.SoPhong,
                    LoaiPhong = pb.LoaiPhong,
                    TrangThai = pb.TrangThai,
                    TienPhong = (decimal)pb.TienPhong
                })
                .ToList();

            DG_PhongBenh.ItemsSource = data;
        }

        public class PhongBenhViewModel
        {
            public int MaPhong { get; set; }
            public string SoPhong { get; set; }
            public string LoaiPhong { get; set; }
            public string TrangThai { get; set; }
            public decimal TienPhong { get; set; }
            public bool IsSelected { get; } 
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.IsChecked == true;

            foreach (PhongBenhViewModel item in DG_PhongBenh.ItemsSource)
                item.IsSelected = isChecked;

            DG_PhongBenh.Items.Refresh();
        }

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            var form = new Form_PhongBenh();
            if (form.ShowDialog() == true)
                LoadData();
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int maPhong))
            {
                using var context = new QLBVEntities();
                var phong = context.PHONGBENHs.Find(maPhong);
                if (phong != null)
                {
                    var form = new Form_PhongBenh(phong);
                    if (form.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void Btn_Xoa_Row_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out int maPhong))
            {
                var phong = db.PHONGBENHs.Find(maPhong);
                if (phong == null)
                {
                    MessageBox.Show("Không tìm thấy phòng bệnh!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Xóa phòng bệnh: {phong.SoPhong}?", "Xác nhận", 
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return;

                try
                {
                    db.PHONGBENHs.Remove(phong);
                    db.SaveChanges();
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Tb_timKiem_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Tb_timKiem.Text == "Tìm kiếm...")
            {
                Tb_timKiem.Text = "";
                Tb_timKiem.Foreground = Brushes.Black;
            }
        }

        private void Tb_timKiem_LostFocus(object sender, RoutedEventArgs e)
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
            if (keyword == "Tìm kiếm...") keyword = "";

            if (string.IsNullOrEmpty(keyword))
            {
                LoadData();
                return;
            }

            using var context = new QLBVEntities();
            IQueryable<PHONGBENH> query = context.PHONGBENHs;

            int selectedIndex = ComboBox_TimKiem.SelectedIndex;
            if (selectedIndex == 0)      query = query.Where(p => p.SoPhong.Contains(keyword));
            else if (selectedIndex == 1) query = query.Where(p => p.LoaiPhong.Contains(keyword));
            else if (selectedIndex == 2) query = query.Where(p => p.TrangThai.Contains(keyword));

            DG_PhongBenh.ItemsSource = query
                .Select(p => new PhongBenhViewModel
                {
                    MaPhong = p.MaPhong,
                    SoPhong = p.SoPhong,
                    LoaiPhong = p.LoaiPhong,
                    TrangThai = p.TrangThai,
                    TienPhong = (decimal)p.TienPhong
                })
                .ToList();
        }
    }
}

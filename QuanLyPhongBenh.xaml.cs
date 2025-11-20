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
    /// Interaction logic for QuanLyPhongBenh.xaml
    /// </summary>
    public partial class QuanLyPhongBenh : Page
    {
        QLBVEntities db = new QLBVEntities();

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
                    TienPhong = (decimal)pb.TienPhong,
                    IsSelected = false
                }).ToList();

            DG_PhongBenh.ItemsSource = data;
        }
        public class PhongBenhViewModel
        {
            public int MaPhong { get; set; }
            public string SoPhong { get; set; }
            public string LoaiPhong { get; set; }
            public string TrangThai { get; set; }
            public decimal TienPhong { get; set; }

            public bool IsSelected { get; set; } = false;
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            foreach (dynamic item in DG_PhongBenh.ItemsSource)
            {
                item.IsSelected = isChecked;
            }

            DG_PhongBenh.Items.Refresh();
        }

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            Form_PhongBenh fpb = new Form_PhongBenh();

            bool? result = fpb.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maPB))
            {
                using (var newDb = new QLBVEntities())
                {
                    var pb = newDb.PHONGBENHs.Find(maPB);
                    if (pb != null)
                    {
                        Form_PhongBenh fpb = new Form_PhongBenh(pb);
                        bool? result = fpb.ShowDialog();
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
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maPB))
            {
                var pb = db.PHONGBENHs.Find(maPB);
                if (pb != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa phòng bệnh: {pb.SoPhong}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.PHONGBENHs.Remove(pb);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể xóa phòng bệnh này. Chi tiết lỗi: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy phòng bệnh để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

            using (var newDb = new QLBVEntities())
            {
                var query = newDb.PHONGBENHs.AsQueryable();

                switch (ComboBox_TimKiem.SelectedIndex)
                {
                    case 0: // Số phòng
                        query = query.Where(pb => pb.SoPhong.Contains(keyword));
                        break;

                    case 1: // Loại phòng
                        query = query.Where(pb => pb.LoaiPhong.Contains(keyword));
                        break;

                    case 2: // Trạng thái
                        query = query.Where(pb => pb.TrangThai.Contains(keyword));
                        break;

                   
                }

                DG_PhongBenh.ItemsSource = query.ToList();
            }
        }

    }
}

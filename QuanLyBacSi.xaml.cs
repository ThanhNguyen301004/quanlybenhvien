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
using System.Globalization;
using System.IO;


namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for QuanLyBacSi.xaml
    /// </summary>
    public partial class QuanLyBacSi : Page
    {
        QLBVEntities db = new QLBVEntities();


        public QuanLyBacSi()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var data = db.BACSIs
                .Select(bs => new BacSiViewModel
                {
                    MaBacSi = bs.MaBacSi,
                    HoTen = bs.HoTen,
                    SoDienThoai = bs.SoDienThoai,
                    Email = bs.Email,
                    ChuyenKhoa = bs.ChuyenKhoa,
                    AnhDaiDien = bs.AnhDaiDien,   
                    IsSelected = false
                }).ToList();

            DG_BacSi.ItemsSource = data;
        }

        public class BacSiViewModel
        {
            public int MaBacSi { get; set; }
            public string HoTen { get; set; }
            public string ChuyenKhoa { get; set; }
            public string SoDienThoai { get; set; }
            public string Email { get; set; }
            public byte[] AnhDaiDien { get; set; }

            public bool IsSelected { get; set; } = false;
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            foreach (dynamic item in DG_BacSi.ItemsSource)
            {
                item.IsSelected = isChecked;
            }

            DG_BacSi.Items.Refresh();
        }

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            Form_BacSi fbs = new Form_BacSi();

            bool? result = fbs.ShowDialog();

            if (result == true)
            {
                LoadData();
            }
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBS))
            {
                using (var newDb = new QLBVEntities())
                {
                    var bs = newDb.BACSIs.Find(maBS);
                    if (bs != null)
                    {
                        Form_BacSi fbs = new Form_BacSi(bs);
                        bool? result = fbs.ShowDialog();
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
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBS))
            {
                var bs = db.BACSIs.Find(maBS);
                if (bs != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa bác sĩ: {bs.HoTen}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.BACSIs.Remove(bs);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể xóa bác sĩ này. Chi tiết lỗi: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy bác sĩ để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm...")
            {
                DG_BacSi.ItemsSource = db.BACSIs.ToList();
                return;
            }
            
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_TimKiem.SelectedItem;
            string selectedField = selectedItem.Content.ToString();

            IQueryable<BACSI> query = db.BACSIs;

            switch (selectedField)
            {
                case "Họ tên":
                    query = query.Where(bs => bs.HoTen.Contains(keyword));
                    break;

                case "Chuyên Khoa":
                    query = query.Where(bs => bs.ChuyenKhoa.Contains(keyword));
                    break;

                case "Số Điện Thoại":
                    query = query.Where(bs => bs.SoDienThoai.Contains(keyword));
                    break;

                case "Email":
                    query = query.Where(bs => bs.Email.Contains(keyword));
                    break;

                default:
                    MessageBox.Show("Vui lòng chọn trường tìm kiếm hợp lệ");
                    return;
            }

            var result = query.ToList();

            if (result.Any())
            {
                DG_BacSi.ItemsSource = result;
            }
            else
            {
                MessageBox.Show("Không tìm thấy bác sĩ phù hợp");
                DG_BacSi.ItemsSource = null;
            }
        }

        
    }
}

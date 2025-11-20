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
    /// Interaction logic for QuanLyTaiKhoan.xaml
    /// </summary>
    public partial class QuanLyTaiKhoan : Page
    {
        QLBVEntities db = new QLBVEntities();

        public QuanLyTaiKhoan()
        {
            InitializeComponent();
            LoadData();
        }

        public class TaiKhoanViewModel
        {
            public string TaiKhoan1 { get; set; }
            public string MatKhau { get; set; }
            public string LoaiTK { get; set; }
            public byte[] AnhCaNhan { get; set; }

            public bool IsSelected { get; set; } = false;
        }

        private void LoadData()
        {
            var data = db.TAIKHOANs
                .Select(tk => new TaiKhoanViewModel
                {
                    TaiKhoan1 = tk.TaiKhoan1,
                    MatKhau = tk.MaKhau,
                    LoaiTK = tk.LoaiTK,
                    AnhCaNhan = tk.AnhCaNhan
                }).ToList();

            DG_TaiKhoan.ItemsSource = data;
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            foreach (dynamic item in DG_TaiKhoan.ItemsSource)
            {
                item.IsSelected = isChecked;
            }

            DG_TaiKhoan.Items.Refresh();
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null)
            {
                string maTK = btn.Tag.ToString();   // vì khóa chính là string

                using (var newDb = new QLBVEntities())
                {
                    var tk = newDb.TAIKHOANs.Find(maTK); // Find theo string
                    if (tk != null)
                    {
                        Form_TaiKhoan fbs = new Form_TaiKhoan(tk);
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
            if (btn?.Tag != null)
            {
                string maTK = btn.Tag.ToString();   // lấy khóa chính dạng string
                var tk = db.TAIKHOANs.Find(maTK);   // EF sẽ tìm theo string key
                if (tk != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa tài khoản: {tk.TaiKhoan1}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.TAIKHOANs.Remove(tk);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể xóa tài khoản này. Chi tiết lỗi: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void Button_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string keyword = Tb_timKiem.Text.Trim();
            if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm...")
            {
                DG_TaiKhoan.ItemsSource = db.TAIKHOANs.ToList();
                return;
            }

            // Lấy lựa chọn trong combobox
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_TimKiem.SelectedItem;
            string selectedField = selectedItem.Content.ToString();

            IQueryable<TAIKHOAN> query = db.TAIKHOANs;

            switch (selectedField)
            {
                case "Tên tài khoản":
                    query = query.Where(bn => bn.TaiKhoan1.Contains(keyword));
                    break;
           
                case "Loại tài khoản":
                    query = query.Where(bn => bn.LoaiTK.Contains(keyword));
                    break;
                    
            }

            DG_TaiKhoan.ItemsSource = query.ToList();
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

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            Form_TaiKhoan fbn = new Form_TaiKhoan();

            bool? result = fbn.ShowDialog();
            if (result == true)
            {
                LoadData();
            }

        }
    }
}

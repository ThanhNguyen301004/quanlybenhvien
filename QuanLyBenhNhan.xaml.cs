using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QUANLYBENHVIEN
{
    public partial class QuanLyBenhNhan : Page
    {
        QLBVEntities db = new QLBVEntities();

        public QuanLyBenhNhan()
        {
            InitializeComponent();
            LoadData();
        }

        public class BenhNhanViewModel
        {
            public int MaBenhNhan { get; set; }
            public string HoTen { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public int? SoLanDen { get; set; }
            public string LoaiBaoHiem { get; set; }
            
            public bool IsSelected { get; set; } = false;
        }

        private void LoadData()
        {
            var data = db.BENHNHANs
                .Select(bn => new BenhNhanViewModel
                {
                    MaBenhNhan = bn.MaBenhNhan,
                    HoTen = bn.HoTen,
                    NgaySinh = bn.NgaySinh,
                    GioiTinh = bn.GioiTinh,
                    SoDienThoai = bn.SoDienThoai,
                    DiaChi = bn.DiaChi,
                    SoLanDen = bn.SoLanDen,
                    LoaiBaoHiem = bn.LoaiBaoHiem,
                    IsSelected = false
                }).ToList();

            DG_BenhNhan.ItemsSource = data;
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox).IsChecked ?? false;

            foreach (dynamic item in DG_BenhNhan.ItemsSource)
            {
                item.IsSelected = isChecked;
            }

            DG_BenhNhan.Items.Refresh();
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBN))
            {
                using (var newDb = new QLBVEntities())
                {
                    var bn = newDb.BENHNHANs.Find(maBN);  
                    if (bn != null)
                    {
                        Form_BenhNhan fbn = new Form_BenhNhan(bn);
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
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBN))
            {
                var bn = db.BENHNHANs.Find(maBN);
                if (bn != null)
                {
                    var confirm = MessageBox.Show(
                        $"Bạn có chắc muốn xóa bệnh nhân: {bn.HoTen}?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (confirm == MessageBoxResult.Yes)
                    {
                        try
                        {
                            db.BENHNHANs.Remove(bn);
                            db.SaveChanges();

                            MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadData(); 
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Không thể xóa bệnh nhân này. Chi tiết lỗi: " + ex.Message,
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy bệnh nhân để xóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string keyword = Tb_timKiem.Text.Trim();
            if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm...")
            {
                DG_BenhNhan.ItemsSource = db.BENHNHANs.ToList();
                return;
            }

            // Lấy lựa chọn trong combobox
            ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_TimKiem.SelectedItem;
            string selectedField = selectedItem.Content.ToString();

            IQueryable<BENHNHAN> query = db.BENHNHANs;

            switch (selectedField)
            {
                case "Họ tên":
                    query = query.Where(bn => bn.HoTen.Contains(keyword));
                    break;

                case "Ngày sinh":
                    if (DateTime.TryParse(keyword, out DateTime ns))
                        query = query.Where(bn => bn.NgaySinh == ns);
                    else
                        MessageBox.Show("Vui lòng nhập đúng định dạng ngày (vd: 01/01/2000)");
                    break;

                case "Giới tính":
                    query = query.Where(bn => bn.GioiTinh.Contains(keyword));
                    break;

                case "Số điện thoại":
                    query = query.Where(bn => bn.SoDienThoai.Contains(keyword));
                    break;

                case "Địa chỉ":
                    query = query.Where(bn => bn.DiaChi.Contains(keyword));
                    break;

                case "Số lần đến":
                    if (int.TryParse(keyword, out int soLan))
                        query = query.Where(bn => bn.SoLanDen == soLan);
                    else
                        MessageBox.Show("Số lần đến phải là số nguyên.");
                    break;

                case "Loại bảo hiểm":
                    query = query.Where(bn => bn.LoaiBaoHiem.Contains(keyword));
                    break;
            }

            DG_BenhNhan.ItemsSource = query.ToList();
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
            Form_BenhNhan fbn = new Form_BenhNhan();
            
            bool? result = fbn.ShowDialog();
            if (result == true) 
            {
                LoadData();
            }
            
        }
    }
}

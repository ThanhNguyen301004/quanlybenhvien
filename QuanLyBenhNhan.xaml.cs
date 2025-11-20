using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace QUANLYBENHVIEN
{
    public partial class QuanLyBenhNhan : Page
    {
        private QLBVEntities db;
        private ObservableCollection<BenhNhanViewModel> benhNhanList;

        public QuanLyBenhNhan()
        {
            InitializeComponent();
            db = new QLBVEntities();
            benhNhanList = new ObservableCollection<BenhNhanViewModel>();
            LoadData();
        }

        // ViewModel with INotifyPropertyChanged for better data binding
        public class BenhNhanViewModel : INotifyPropertyChanged
        {
            private bool _isSelected;

            public int MaBenhNhan { get; set; }
            public string HoTen { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public int? SoLanDen { get; set; }
            public string LoaiBaoHiem { get; set; }
            
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        OnPropertyChanged(nameof(IsSelected));
                    }
                }
            }

            // Hiển thị tuổi
            public int Tuoi => DateTime.Now.Year - NgaySinh.Year;
            
            // Format ngày sinh
            public string NgaySinhFormat => NgaySinh.ToString("dd/MM/yyyy");

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void LoadData()
        {
            try
            {
                // Sử dụng AsNoTracking để tăng hiệu suất khi chỉ đọc dữ liệu
                var data = db.BENHNHANs
                    .AsNoTracking()
                    .OrderByDescending(bn => bn.MaBenhNhan)
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

                benhNhanList.Clear();
                foreach (var item in data)
                {
                    benhNhanList.Add(item);
                }

                DG_BenhNhan.ItemsSource = benhNhanList;
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStatusBar()
        {
            // Cập nhật số lượng bệnh nhân (nếu có StatusBar trong XAML)
            int total = benhNhanList.Count;
            int selected = benhNhanList.Count(bn => bn.IsSelected);
            // StatusText.Text = $"Tổng: {total} | Đã chọn: {selected}";
        }

        private void ChkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.IsChecked ?? false;

            foreach (var item in benhNhanList)
            {
                item.IsSelected = isChecked;
            }
            
            UpdateStatusBar();
        }

        private void Btn_Sua_Row_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBN))
            {
                try
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
                        else
                        {
                            MessageBox.Show("Không tìm thấy bệnh nhân!", "Thông báo", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi sửa: {ex.Message}", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
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
                        $"Bạn có chắc muốn xóa bệnh nhân: {bn.HoTen}?\n\n" +
                        $"Mã BN: {bn.MaBenhNhan}\n" +
                        $"SĐT: {bn.SoDienThoai}",
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

                            MessageBox.Show("Xóa thành công!", "Thông báo", 
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Không thể xóa bệnh nhân này.\n\n" +
                                $"Chi tiết: {ex.Message}\n\n" +
                                $"(Có thể bệnh nhân đang có dữ liệu liên quan)", 
                                "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy bệnh nhân để xóa!", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Xóa nhiều bệnh nhân
        private void Btn_XoaNhieu_Click(object sender, RoutedEventArgs e)
        {
            var selected = benhNhanList.Where(bn => bn.IsSelected).ToList();
            
            if (selected.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một bệnh nhân để xóa!", 
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa {selected.Count} bệnh nhân đã chọn?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirm == MessageBoxResult.Yes)
            {
                int successCount = 0;
                int failCount = 0;

                foreach (var item in selected)
                {
                    try
                    {
                        var bn = db.BENHNHANs.Find(item.MaBenhNhan);
                        if (bn != null)
                        {
                            db.BENHNHANs.Remove(bn);
                            successCount++;
                        }
                    }
                    catch
                    {
                        failCount++;
                    }
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show($"Xóa thành công {successCount} bệnh nhân!" +
                        (failCount > 0 ? $"\nKhông thể xóa {failCount} bệnh nhân." : ""),
                        "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            string keyword = Tb_timKiem.Text.Trim();
            
            // Nếu không có từ khóa, load lại toàn bộ
            if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm...")
            {
                LoadData();
                return;
            }

            try
            {
                ComboBoxItem selectedItem = (ComboBoxItem)ComboBox_TimKiem.SelectedItem;
                string selectedField = selectedItem?.Content.ToString() ?? "Họ tên";

                IQueryable<BENHNHAN> query = db.BENHNHANs;

                switch (selectedField)
                {
                    case "Tất cả":
                        query = query.Where(bn => 
                            bn.HoTen.Contains(keyword) ||
                            bn.SoDienThoai.Contains(keyword) ||
                            bn.DiaChi.Contains(keyword) ||
                            bn.GioiTinh.Contains(keyword) ||
                            bn.LoaiBaoHiem.Contains(keyword)
                        );
                        break;

                    case "Họ tên":
                        query = query.Where(bn => bn.HoTen.Contains(keyword));
                        break;

                    case "Ngày sinh":
                        if (DateTime.TryParse(keyword, out DateTime ns))
                            query = query.Where(bn => bn.NgaySinh == ns);
                        else
                        {
                            MessageBox.Show("Vui lòng nhập đúng định dạng ngày (dd/MM/yyyy)", 
                                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
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
                        {
                            MessageBox.Show("Số lần đến phải là số nguyên.", "Thông báo", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        break;

                    case "Loại bảo hiểm":
                        query = query.Where(bn => bn.LoaiBaoHiem.Contains(keyword));
                        break;
                }

                var results = query
                    .AsNoTracking()
                    .OrderByDescending(bn => bn.MaBenhNhan)
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

                benhNhanList.Clear();
                foreach (var item in results)
                {
                    benhNhanList.Add(item);
                }

                if (results.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy kết quả nào!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Tìm kiếm khi nhấn Enter
        private void Tb_timKiem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                PerformSearch();
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

        private void Btn_Them_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Form_BenhNhan fbn = new Form_BenhNhan();
                bool? result = fbn.ShowDialog();
                
                if (result == true)
                {
                    LoadData();
                    MessageBox.Show("Thêm bệnh nhân thành công!", "Thông báo", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Làm mới dữ liệu
        private void Btn_LamMoi_Click(object sender, RoutedEventArgs e)
        {
            Tb_timKiem.Text = "Tìm kiếm...";
            Tb_timKiem.Foreground = Brushes.Gray;
            if (ComboBox_TimKiem.Items.Count > 0)
                ComboBox_TimKiem.SelectedIndex = 0;
            
            LoadData();
        }

        // Export to Excel (cần thêm thư viện EPPlus hoặc ClosedXML)
        private void Btn_XuatExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Chức năng xuất Excel đang được phát triển!", "Thông báo", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                // Implement Excel export logic here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xem chi tiết bệnh nhân
        private void Btn_XemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn?.Tag != null && int.TryParse(btn.Tag.ToString(), out int maBN))
            {
                var bn = db.BENHNHANs.Find(maBN);
                if (bn != null)
                {
                    string info = $"THÔNG TIN BỆNH NHÂN\n\n" +
                        $"Mã BN: {bn.MaBenhNhan}\n" +
                        $"Họ tên: {bn.HoTen}\n" +
                        $"Ngày sinh: {bn.NgaySinh:dd/MM/yyyy}\n" +
                        $"Giới tính: {bn.GioiTinh}\n" +
                        $"SĐT: {bn.SoDienThoai}\n" +
                        $"Địa chỉ: {bn.DiaChi}\n" +
                        $"Số lần đến: {bn.SoLanDen}\n" +
                        $"Loại bảo hiểm: {bn.LoaiBaoHiem}";

                    MessageBox.Show(info, "Chi tiết bệnh nhân", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        // Dispose DbContext khi đóng page
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}

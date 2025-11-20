using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace QUANLYBENHVIEN
{
    public partial class Form_BenhNhan : Window
    {
        private readonly QLBVEntities db;
        private readonly BENHNHAN editingBenhNhan;
        private readonly bool isEditMode;

        // Constructor cho chế độ thêm mới
        public Form_BenhNhan()
        {
            InitializeComponent();
            db = new QLBVEntities();
            isEditMode = false;
            SetupUI();
        }

        // Constructor cho chế độ chỉnh sửa
        public Form_BenhNhan(BENHNHAN bn) : this()
        {
            if (bn == null)
                throw new ArgumentNullException(nameof(bn));

            editingBenhNhan = bn;
            isEditMode = true;
            LoadBenhNhanData();
        }

        private void SetupUI()
        {
            Title = isEditMode ? "Chỉnh sửa thông tin bệnh nhân" : "Thêm bệnh nhân mới";
            
            // Giới hạn ngày sinh (không quá 150 tuổi, không tương lai)
            dp_ngaySinh.DisplayDateEnd = DateTime.Today;
            dp_ngaySinh.DisplayDateStart = DateTime.Today.AddYears(-150);

            // Thêm validation cho số điện thoại
            Tb_soDienThoai.PreviewTextInput += ValidatePhoneNumber;
            Tb_soDienThoai.MaxLength = 12;
        }

        private void LoadBenhNhanData()
        {
            if (editingBenhNhan == null) return;

            Tb_hoTen.Text = editingBenhNhan.HoTen;
            dp_ngaySinh.SelectedDate = editingBenhNhan.NgaySinh;
            SetGioiTinh(editingBenhNhan.GioiTinh);
            Tb_soDienThoai.Text = editingBenhNhan.SoDienThoai;
            Tb_diaChi.Text = editingBenhNhan.DiaChi;
            SelectLoaiBaoHiem(editingBenhNhan.LoaiBaoHiem);
        }

        private void SetGioiTinh(string gioiTinh)
        {
            switch (gioiTinh)
            {
                case "Nam":
                    rb_nam.IsChecked = true;
                    break;
                case "Nữ":
                    rb_nu.IsChecked = true;
                    break;
                default:
                    rb_khac.IsChecked = true;
                    break;
            }
        }

        private void SelectLoaiBaoHiem(string loaiBaoHiem)
        {
            if (string.IsNullOrEmpty(loaiBaoHiem)) return;

            foreach (ComboBoxItem item in ComboBox_loaiBaoHiem.Items)
            {
                if (item.Content.ToString() == loaiBaoHiem)
                {
                    ComboBox_loaiBaoHiem.SelectedItem = item;
                    break;
                }
            }
        }

        private void ValidatePhoneNumber(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Chỉ cho phép nhập số
            e.Handled = !IsTextNumeric(e.Text);
        }

        private bool IsTextNumeric(string text)
        {
            return Regex.IsMatch(text, "^[0-9]+$");
        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate dữ liệu
                if (!ValidateInput(out string errorMessage))
                {
                    MessageBox.Show(errorMessage, "Lỗi nhập liệu", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Lấy dữ liệu từ form
                var benhNhanData = GetBenhNhanDataFromForm();

                // Thực hiện thêm hoặc sửa
                if (isEditMode)
                {
                    UpdateBenhNhan(benhNhanData);
                }
                else
                {
                    AddNewBenhNhan(benhNhanData);
                }

                // Lưu thay đổi
                db.SaveChanges();

                MessageBox.Show(
                    isEditMode ? "Cập nhật thông tin bệnh nhân thành công!" 
                               : "Thêm bệnh nhân mới thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                
                MessageBox.Show(
                    "Lỗi validation: " + string.Join(Environment.NewLine, errorMessages),
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Đã xảy ra lỗi: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool ValidateInput(out string errorMessage)
        {
            errorMessage = string.Empty;

            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(Tb_hoTen.Text))
            {
                errorMessage = "Vui lòng nhập họ tên bệnh nhân!";
                Tb_hoTen.Focus();
                return false;
            }

            if (Tb_hoTen.Text.Length < 2)
            {
                errorMessage = "Họ tên phải có ít nhất 2 ký tự!";
                Tb_hoTen.Focus();
                return false;
            }

            // Kiểm tra ngày sinh
            if (!dp_ngaySinh.SelectedDate.HasValue)
            {
                errorMessage = "Vui lòng chọn ngày sinh!";
                dp_ngaySinh.Focus();
                return false;
            }

            var age = DateTime.Today.Year - dp_ngaySinh.SelectedDate.Value.Year;
            if (dp_ngaySinh.SelectedDate.Value > DateTime.Today.AddYears(-age))
                age--;

            if (age < 0 || age > 150)
            {
                errorMessage = "Ngày sinh không hợp lệ!";
                dp_ngaySinh.Focus();
                return false;
            }

            // Kiểm tra giới tính
            if (rb_nam.IsChecked != true && rb_nu.IsChecked != true && rb_khac.IsChecked != true)
            {
                errorMessage = "Vui lòng chọn giới tính!";
                return false;
            }

            // Kiểm tra số điện thoại
            if (string.IsNullOrWhiteSpace(Tb_soDienThoai.Text))
            {
                errorMessage = "Vui lòng nhập số điện thoại!";
                Tb_soDienThoai.Focus();
                return false;
            }

            if (!Regex.IsMatch(Tb_soDienThoai.Text, @"^0\d{9,10}$"))
            {
                errorMessage = "Số điện thoại không đúng định dạng! (Phải bắt đầu bằng 0 và có 10-11 số)";
                Tb_soDienThoai.Focus();
                return false;
            }

            // Kiểm tra địa chỉ
            if (string.IsNullOrWhiteSpace(Tb_diaChi.Text))
            {
                errorMessage = "Vui lòng nhập địa chỉ!";
                Tb_diaChi.Focus();
                return false;
            }

            return true;
        }

        private BenhNhanDTO GetBenhNhanDataFromForm()
        {
            return new BenhNhanDTO
            {
                HoTen = Tb_hoTen.Text.Trim(),
                NgaySinh = dp_ngaySinh.SelectedDate.Value,
                GioiTinh = rb_nam.IsChecked == true ? "Nam" :
                          rb_nu.IsChecked == true ? "Nữ" : "Khác",
                SoDienThoai = Tb_soDienThoai.Text.Trim(),
                DiaChi = Tb_diaChi.Text.Trim(),
                LoaiBaoHiem = (ComboBox_loaiBaoHiem.SelectedItem as ComboBoxItem)?.Content.ToString()
            };
        }

        private void AddNewBenhNhan(BenhNhanDTO data)
        {
            var newBN = new BENHNHAN
            {
                HoTen = data.HoTen,
                NgaySinh = data.NgaySinh,
                GioiTinh = data.GioiTinh,
                SoDienThoai = data.SoDienThoai,
                DiaChi = data.DiaChi,
                LoaiBaoHiem = data.LoaiBaoHiem,
                SoLanDen = 1,
                NgayTao = DateTime.Now
            };

            db.BENHNHANs.Add(newBN);
        }

        private void UpdateBenhNhan(BenhNhanDTO data)
        {
            var bn = db.BENHNHANs.Find(editingBenhNhan.MaBenhNhan);
            
            if (bn == null)
            {
                throw new InvalidOperationException("Không tìm thấy bệnh nhân cần cập nhật!");
            }

            bn.HoTen = data.HoTen;
            bn.NgaySinh = data.NgaySinh;
            bn.GioiTinh = data.GioiTinh;
            bn.SoDienThoai = data.SoDienThoai;
            bn.DiaChi = data.DiaChi;
            bn.LoaiBaoHiem = data.LoaiBaoHiem;
            bn.NgayCapNhat = DateTime.Now;
        }

        private void Button_Huy_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy? Các thay đổi sẽ không được lưu.",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            db?.Dispose();
            base.OnClosed(e);
        }

        // DTO class để truyền dữ liệu
        private class BenhNhanDTO
        {
            public string HoTen { get; set; }
            public DateTime NgaySinh { get; set; }
            public string GioiTinh { get; set; }
            public string SoDienThoai { get; set; }
            public string DiaChi { get; set; }
            public string LoaiBaoHiem { get; set; }
        }
    }
}

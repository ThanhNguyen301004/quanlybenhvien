using System;
using System.Linq;
using System.Windows;

namespace QUANLYBENHVIEN
{
    public partial class Form_HoaDonVienPhi : Window
    {
        QLBVEntities db = new QLBVEntities();
        private HOADONVIENPHI editingHD;

        public Form_HoaDonVienPhi()
        {
            InitializeComponent();

            Cb_benhNhan.ItemsSource = db.BENHNHANs.ToList();
            Cb_phongBenh.ItemsSource = db.PHONGBENHs.ToList();
            
            Cb_benhNhan.SelectionChanged += RecalculateThanhTien;
            Cb_phongBenh.SelectionChanged += RecalculateThanhTien;
            dp_ngayNhap.SelectedDateChanged += RecalculateThanhTien;
            dp_ngayXuat.SelectedDateChanged += RecalculateThanhTien;
        }

        public Form_HoaDonVienPhi(HOADONVIENPHI hd) : this()
        {
            editingHD = hd;

            Cb_benhNhan.SelectedValue = hd.MaBenhNhan;
            Cb_phongBenh.SelectedValue = hd.MaPhong;
            dp_ngayNhap.SelectedDate = hd.NgayNhapVien;
            dp_ngayXuat.SelectedDate = hd.NgayXuatVien;
            Tb_thanhTien.Text = hd.ThanhTien.HasValue ? hd.ThanhTien.Value.ToString("N0") : "0";

        }

        private void RecalculateThanhTien(object sender, EventArgs e)
        {
            try
            {
                if (Cb_benhNhan.SelectedValue is int maBN &&
                    Cb_phongBenh.SelectedValue is int maPB &&
                    dp_ngayNhap.SelectedDate.HasValue &&
                    dp_ngayXuat.SelectedDate.HasValue)
                {
                    var tien = TinhThanhTien(maBN, maPB, dp_ngayNhap.SelectedDate.Value, dp_ngayXuat.SelectedDate.Value);
                    Tb_thanhTien.Text = tien.HasValue ? tien.Value.ToString("N0") : "0";

                }
            }
            catch
            {
                Tb_thanhTien.Text = "0";
            }
        }

        private decimal? TinhThanhTien(int maBenhNhan, int maPhong, DateTime ngayNhap, DateTime ngayXuat)
        {
            BENHNHAN benhNhan = db.BENHNHANs.Find(maBenhNhan);
            PHONGBENH phong = db.PHONGBENHs.Find(maPhong);

            if (benhNhan == null || phong == null)
                throw new Exception("Không tìm thấy bệnh nhân hoặc phòng bệnh.");

            int soNgay = (ngayXuat - ngayNhap).Days;
            if (soNgay <= 0) soNgay = 1;

            decimal? tienPhong = phong.TienPhong * soNgay;

            switch (benhNhan.LoaiBaoHiem)
            {
                case "BHYT":
                    tienPhong += 50000;
                    break;
                case "BHXH":
                    tienPhong += 100000;
                    break;
                default:
                    tienPhong += 150000;
                    break;
            }

            return tienPhong;
        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(Cb_benhNhan.SelectedValue is int maBN) ||
                    !(Cb_phongBenh.SelectedValue is int maPB) ||
                    !dp_ngayNhap.SelectedDate.HasValue ||
                    !dp_ngayXuat.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                decimal? thanhTien = TinhThanhTien(maBN, maPB, dp_ngayNhap.SelectedDate.Value, dp_ngayXuat.SelectedDate.Value);

                if (editingHD == null) // Thêm mới
                {
                    HOADONVIENPHI newHD = new HOADONVIENPHI
                    {
                        MaBenhNhan = maBN,
                        MaPhong = maPB,
                        NgayNhapVien = dp_ngayNhap.SelectedDate.Value,
                        NgayXuatVien = dp_ngayXuat.SelectedDate,
                        ThanhTien = thanhTien
                    };
                    db.HOADONVIENPHIs.Add(newHD);
                }
                else // Sửa
                {
                    var hd = db.HOADONVIENPHIs.Find(editingHD.MaHoaDon);
                    if (hd != null)
                    {
                        hd.MaBenhNhan = maBN;
                        hd.MaPhong = maPB;
                        hd.NgayNhapVien = dp_ngayNhap.SelectedDate.Value;
                        hd.NgayXuatVien = dp_ngayXuat.SelectedDate;
                        hd.ThanhTien = thanhTien;
                    }
                }

                db.SaveChanges();
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void Button_Huy_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

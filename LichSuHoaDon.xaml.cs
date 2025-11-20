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
    public partial class LichSuHoaDon : Page
    {
        QLBVEntities db = new QLBVEntities();

        public LichSuHoaDon()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DG_HOADONVIENPHI.ItemsSource = db.HOADONVIENPHIs.ToList();

        }

        private void Btn_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new QLBVEntities())
            {
                var query = db.HOADONVIENPHIs
                    .Include("BENHNHAN")
                    .Include("PHONGBENH")
                    .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(Tb_maHoaDon.Text))
                {
                    if (int.TryParse(Tb_maHoaDon.Text.Trim(), out int maHD))
                    {
                        query = query.Where(hd => hd.MaHoaDon == maHD);
                    }
                    else
                    {
                        MessageBox.Show("Mã hóa đơn phải là số!");
                        return;
                    }
                }

                
                if (!string.IsNullOrWhiteSpace(Tb_soDienThoai.Text))
                {
                    string sdt = Tb_soDienThoai.Text.Trim();
                    query = query.Where(hd => hd.BENHNHAN.SoDienThoai.Contains(sdt));
                }
                if (dp_tuNgay.SelectedDate != null && dp_denNgay.SelectedDate != null)
                {
                    DateTime tuNgay = dp_tuNgay.SelectedDate.Value.Date;
                    DateTime denNgay = dp_denNgay.SelectedDate.Value.Date;
                    query = query.Where(hd => hd.NgayXuatVien >= tuNgay && hd.NgayXuatVien <= denNgay);
                }

                DG_HOADONVIENPHI.ItemsSource = query.ToList();
            }
        }
        
        private void Btn_XemChiTiet_Click(object sender, RoutedEventArgs e)
        {
            if (!(DG_HOADONVIENPHI.SelectedItem is HOADONVIENPHI selectedHD))
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để xem chi tiết!");
                return;
            }
            
            string info = $"Mã HĐ: {selectedHD.MaHoaDon}\n" +
                          $"Bệnh nhân: {selectedHD.BENHNHAN?.HoTen}\n" +
                          $"SĐT: {selectedHD.BENHNHAN?.SoDienThoai}\n" +
                          $"Phòng: {selectedHD.PHONGBENH?.SoPhong}\n" +
                          $"Ngày nhập viện: {selectedHD.NgayNhapVien:dd/MM/yyyy}\n" +
                          $"Ngày xuất viện: {selectedHD.NgayXuatVien:dd/MM/yyyy}\n" +
                          $"Thành tiền: {selectedHD.ThanhTien:N0} VNĐ";

            MessageBox.Show(info, "Chi tiết hóa đơn");
        }
        
        private void Btn_LamMoi_Click(object sender, RoutedEventArgs e)
        {
            Tb_maHoaDon.Clear();
            Tb_soDienThoai.Clear();
            dp_tuNgay.SelectedDate = DateTime.Now;
            dp_denNgay.SelectedDate = DateTime.Now;

            LoadData();
        }
    }
}

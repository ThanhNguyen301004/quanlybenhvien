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
using System.IO;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for InHoaDon.xaml
    /// </summary>
    public partial class InHoaDon : Page
    {

        QLBVEntities db = new QLBVEntities();

        public InHoaDon()
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
            if (!string.IsNullOrWhiteSpace(Tb_maHoaDon.Text))
            {
                if (int.TryParse(Tb_maHoaDon.Text.Trim(), out int maHD))
                {
                    using (var db = new QLBVEntities())
                    {
                        var hoaDon = db.HOADONVIENPHIs
                            .Include("BENHNHAN")
                            .Include("PHONGBENH")
                            .Where(hd => hd.MaHoaDon == maHD)
                            .ToList();

                        DG_HOADONVIENPHI.ItemsSource = hoaDon;
                    }
                }
                else
                {
                    MessageBox.Show("Mã hóa đơn phải là số!");
                }
            }
            else
            {
                LoadData();
            }
        }
        
        private void Btn_InHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (!(DG_HOADONVIENPHI.SelectedItem is HOADONVIENPHI selectedHD))
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn để in!");
                return;
            }

            string fileName = $"HoaDon_{selectedHD.MaHoaDon}.txt";

            using (StreamWriter sw = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
            {
                sw.WriteLine("===== HÓA ĐƠN VIỆN PHÍ =====");
                sw.WriteLine($"Mã hóa đơn  : {selectedHD.MaHoaDon}");
                sw.WriteLine($"Bệnh nhân   : {selectedHD.BENHNHAN?.HoTen}");
                sw.WriteLine($"SĐT         : {selectedHD.BENHNHAN?.SoDienThoai}");
                sw.WriteLine($"Phòng bệnh  : {selectedHD.PHONGBENH?.SoPhong}");
                sw.WriteLine($"Ngày nhập viện : {selectedHD.NgayNhapVien:dd/MM/yyyy}");
                sw.WriteLine($"Ngày xuất viện : {selectedHD.NgayXuatVien:dd/MM/yyyy}");
                sw.WriteLine($"Thành tiền  : {selectedHD.ThanhTien:N0} VNĐ");
                sw.WriteLine("============================");
            }

            MessageBox.Show($"Đã in hóa đơn ra file: {fileName}");
        }

    }
}

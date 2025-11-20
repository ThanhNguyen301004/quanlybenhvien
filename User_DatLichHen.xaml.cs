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
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for User_DatLichHen.xaml
    /// </summary>
    public partial class User_DatLichHen : Window
    {
        
        public User_DatLichHen()
        {
            InitializeComponent();
        }

        private BACSI doctor;
        public User_DatLichHen(BACSI selectedDoctor)
        {
            InitializeComponent();
            doctor = selectedDoctor;
        }
        


        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new QLBVEntities())
                {
                    // 1. Lấy thông tin từ form
                    string hoTen = Tb_hoTen.Text.Trim();
                    DateTime? ngaySinh = dp_ngaySinh.SelectedDate;
                    string gioiTinh = rb_nam.IsChecked == true ? "Nam" :
                                      rb_nu.IsChecked == true ? "Nữ" : "Khác";
                    string soDienThoai = Tb_soDienThoai.Text.Trim();
                    string diaChi = Tb_diaChi.Text.Trim();
                    string loaiBaoHiem = (ComboBox_loaiBaoHiem.SelectedItem as ComboBoxItem)?.Content.ToString();
                    DateTime? ngayHen = dp_ngayDatLich.SelectedDate;
                    string mucDich = Tb_mucDich.Text.Trim();
                    string email = Tb_email.Text.Trim();

                    if (string.IsNullOrEmpty(hoTen) || ngaySinh == null || ngayHen == null)
                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    
                    BENHNHAN benhNhan = db.BENHNHANs.FirstOrDefault(
                        b => b.SoDienThoai == soDienThoai && b.HoTen == hoTen && b.NgaySinh == ngaySinh);

                    if (benhNhan == null)
                    {
                        // Bệnh nhân mới
                        benhNhan = new BENHNHAN
                        {
                            HoTen = hoTen,
                            NgaySinh = ngaySinh.Value,
                            GioiTinh = gioiTinh,
                            SoDienThoai = soDienThoai,
                            DiaChi = diaChi,
                            LoaiBaoHiem = loaiBaoHiem,
                            SoLanDen = 0
                        };

                        db.BENHNHANs.Add(benhNhan);
                        db.SaveChanges();
                    }

                    // 3. Thêm lịch hẹn
                    LICHHEN lichHen = new LICHHEN
                    {
                        MaBenhNhan = benhNhan.MaBenhNhan,
                        MaBacSi = doctor.MaBacSi,   // doctor phải được truyền từ constructor
                        NgayHen = ngayHen.Value,
                        MucDich = mucDich,
                        TrangThai = "Chờ xác nhận",
                        Email = email
                    };

                    db.LICHHENs.Add(lichHen);
                    db.SaveChanges();
                    
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đặt lịch hẹn: " + ex.Message, "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Button_Huy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

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

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for Form_LichHen.xaml
    /// </summary>
    public partial class Form_LichHen : Window
    {
        QLBVEntities db = new QLBVEntities();

        private LICHHEN editingLH;

        public Form_LichHen()
        {
            InitializeComponent();

            Cb_benhNhan.ItemsSource = db.BENHNHANs.ToList();
            Cb_bacSi.ItemsSource = db.BACSIs.ToList();
        }

        public Form_LichHen(LICHHEN lh) : this()
        {
            editingLH = lh;

            Cb_benhNhan.SelectedValue = lh.MaBenhNhan;
            Cb_bacSi.SelectedValue = lh.MaBacSi;
            dp_ngayHen.SelectedDate = lh.NgayHen;
            Tb_mucDich.Text = lh.MucDich;
            foreach (ComboBoxItem item in ComboBox_TrangThai.Items)
            {
                if (item.Content.ToString() == lh.TrangThai)
                {
                    ComboBox_TrangThai.SelectedItem = item;
                    break;
                }
            }
            Tb_email.Text = lh.Email;
        }

        private string LayTrangThai()
        {
            if (ComboBox_TrangThai.SelectedItem != null)
            {
                ComboBoxItem item = ComboBox_TrangThai.SelectedItem as ComboBoxItem;
                return item.Content.ToString();
            }
            return "";
        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(Cb_benhNhan.SelectedValue is int maBN) ||
                    !(Cb_bacSi.SelectedValue is int maBS) ||
                    !dp_ngayHen.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                string trangThai = LayTrangThai();
                string email = Tb_email.Text.Trim();
                DateTime ngayHen = dp_ngayHen.SelectedDate.Value;
                string mucDich = Tb_mucDich.Text.Trim();

                BENHNHAN bn = db.BENHNHANs.Find(maBN);
                BACSI bs = db.BACSIs.Find(maBS);

                LICHHEN lh;

                if (editingLH == null) // ➕ Thêm mới
                {
                    lh = new LICHHEN
                    {
                        MaBenhNhan = maBN,
                        MaBacSi = maBS,
                        NgayHen = ngayHen,
                        MucDich = mucDich,
                        TrangThai = trangThai,
                        Email = email
                    };
                    db.LICHHENs.Add(lh);
                }
                else // ✏️ Sửa
                {
                    lh = db.LICHHENs.Find(editingLH.MaLichHen);
                    if (lh != null)
                    {
                        lh.MaBenhNhan = maBN;
                        lh.MaBacSi = maBS;
                        lh.NgayHen = ngayHen;
                        lh.MucDich = mucDich;
                        if (trangThai == "Đã Lên Lịch" && lh.TrangThai != "Đã Lên Lịch")
                        {
                            string subject = editingLH == null
                       ? "Xác nhận đặt lịch hẹn khám bệnh"
                       : "Cập nhật lịch hẹn khám bệnh";

                            string body = $"Xin chào {bn.HoTen},<br/><br/>" +
                                          $"Lịch hẹn của bạn với bác sĩ <b>{bs.HoTen}</b> " +
                                          $"vào ngày <b>{ngayHen:dd/MM/yyyy}</b> đã được {(editingLH == null ? "tạo mới" : "cập nhật")}.<br/>" +
                                          $"Mục đích: {mucDich}<br/>" +
                                          $"Trạng thái hiện tại: <b>{trangThai}</b><br/><br/>" +
                                          $"Xin cảm ơn,<br/>Phòng khám";

                            EmailHelper.SendEmail(email, subject, body);
                        }
                        Console.WriteLine("fasdhb");
                        lh.TrangThai = trangThai;
                        lh.Email = email;

                        if (lh.TrangThai == "Hoàn Thành")
                        {
                            BENHNHAN benhNhan = db.BENHNHANs.Find(lh.MaBenhNhan);
                            if (benhNhan != null)
                                benhNhan.SoLanDen += 1;
                        }

                        
                    }
                }

                db.SaveChanges();


                MessageBox.Show("Lưu lịch hẹn thành công!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);

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

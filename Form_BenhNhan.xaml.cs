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
    public partial class Form_BenhNhan : Window
    {
        QLBVEntities db = new QLBVEntities();

        private BENHNHAN editingBenhNhan;

        public Form_BenhNhan()
        {
            InitializeComponent();
        }

        public Form_BenhNhan(BENHNHAN bn) : this()
        {
            editingBenhNhan = bn;

            Tb_hoTen.Text = bn.HoTen;
            dp_ngaySinh.SelectedDate = bn.NgaySinh;

            if (bn.GioiTinh == "Nam") rb_nam.IsChecked = true;
            else if (bn.GioiTinh == "Nữ") rb_nu.IsChecked = true;
            else rb_khac.IsChecked = true;

            Tb_soDienThoai.Text = bn.SoDienThoai;
            Tb_diaChi.Text = bn.DiaChi;
            
            foreach (ComboBoxItem item in ComboBox_loaiBaoHiem.Items)
            {
                if (item.Content.ToString() == bn.LoaiBaoHiem)
                {
                    ComboBox_loaiBaoHiem.SelectedItem = item;
                    break;
                }
            }
        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hoTen = Tb_hoTen.Text.Trim();
                DateTime? ngaySinh = dp_ngaySinh.SelectedDate;
                string gioiTinh = rb_nam.IsChecked == true ? "Nam" :
                                  rb_nu.IsChecked == true ? "Nữ" : "Khác";
                string soDienThoai = Tb_soDienThoai.Text.Trim();
                string diaChi = Tb_diaChi.Text.Trim();
                string loaiBaoHiem = (ComboBox_loaiBaoHiem.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrEmpty(hoTen) || ngaySinh == null || string.IsNullOrEmpty(soDienThoai))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                if (editingBenhNhan == null) // Thêm mới
                {
                    BENHNHAN newBN = new BENHNHAN
                    {
                        HoTen = hoTen,
                        NgaySinh = ngaySinh.Value,
                        GioiTinh = gioiTinh,
                        SoDienThoai = soDienThoai,
                        DiaChi = diaChi,
                        LoaiBaoHiem = loaiBaoHiem,
                        SoLanDen = 1
                    };

                    db.BENHNHANs.Add(newBN);
                }
                else // Sửa
                {
                    var bn = db.BENHNHANs.Find(editingBenhNhan.MaBenhNhan);
                    if (bn != null)
                    {
                        bn.HoTen = hoTen;
                        bn.NgaySinh = ngaySinh.Value;
                        bn.GioiTinh = gioiTinh;
                        bn.SoDienThoai = soDienThoai;
                        bn.DiaChi = diaChi;
                        bn.LoaiBaoHiem = loaiBaoHiem;
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


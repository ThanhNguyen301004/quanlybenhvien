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
    /// Interaction logic for Form_PhongBenh.xaml
    /// </summary>
    public partial class Form_PhongBenh : Window
    {
        QLBVEntities db = new QLBVEntities();

        private PHONGBENH editingPhongBenh;
        public Form_PhongBenh()
        {
            InitializeComponent();
        }

        public Form_PhongBenh(PHONGBENH pb) : this()
        {
            editingPhongBenh = pb;

            Tb_soPhong.Text = pb.SoPhong;
            Tb_tienPhong.Text = pb.TienPhong.ToString();
            
            foreach (ComboBoxItem item in ComboBox_loaiPhong.Items)
            {
                if (item.Content.ToString() == pb.LoaiPhong)
                {
                    ComboBox_loaiPhong.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in ComboBox_trangThai.Items)
            {
                if (item.Content.ToString() == pb.TrangThai)
                {
                    ComboBox_trangThai.SelectedItem = item;
                    break;
                }
            }
        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string soPhong = Tb_soPhong.Text.Trim();
                decimal tienPhong = decimal.Parse(Tb_tienPhong.Text.Trim());
                
                string loaiPhong = (ComboBox_loaiPhong.SelectedItem as ComboBoxItem)?.Content.ToString();
                string trangThai = (ComboBox_trangThai.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (string.IsNullOrEmpty(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }

                if (editingPhongBenh == null) // Thêm mới
                {
                    PHONGBENH newPB = new PHONGBENH
                    {
                        SoPhong = soPhong,
                        TienPhong = tienPhong,
                        LoaiPhong = loaiPhong,
                        TrangThai = trangThai
                    };

                    db.PHONGBENHs.Add(newPB);
                }
                else 
                {
                    var pb = db.PHONGBENHs.Find(editingPhongBenh.MaPhong);
                    if (pb != null)
                    {
                        pb.SoPhong = soPhong;
                        pb.TienPhong = tienPhong;
                        pb.LoaiPhong = loaiPhong;
                        pb.TrangThai = trangThai;
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

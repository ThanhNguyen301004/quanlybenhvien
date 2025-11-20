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
using Microsoft.Win32;
using System.IO;


namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for Form_BacSi.xaml
    /// </summary>
    public partial class Form_BacSi : Window
    {
        QLBVEntities db = new QLBVEntities();
        private byte[] selectedAvatarBytes;
        private BACSI editingBacSi;

        public Form_BacSi()
        {
            InitializeComponent();
        }

        public Form_BacSi(BACSI bs) : this()
        {
            editingBacSi = bs;

            Tb_hoTen.Text = bs.HoTen;
            Tb_soDienThoai.Text = bs.SoDienThoai;
            Tb_email.Text = bs.Email;

            foreach (ComboBoxItem item in ComboBox_chuyenKhoa.Items)
            {
                if (item.Content.ToString() == bs.ChuyenKhoa)
                {
                    ComboBox_chuyenKhoa.SelectedItem = item;
                    break;
                }
            }

            if (bs.AnhDaiDien != null)
            {
                using (var ms = new MemoryStream(bs.AnhDaiDien))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    imgAvatar.Source = bitmap;
                }
            }

        }

        private void Button_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string hoTen = Tb_hoTen.Text.Trim();
                string soDienThoai = Tb_soDienThoai.Text.Trim();
                string email = Tb_email.Text.Trim();
                string chuyenKhoa = (ComboBox_chuyenKhoa.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (editingBacSi == null) // Thêm mới
                {
                    BACSI newBS = new BACSI
                    {
                        HoTen = hoTen,
                        SoDienThoai = soDienThoai,
                        Email = email,
                        ChuyenKhoa = chuyenKhoa,
                        AnhDaiDien = selectedAvatarBytes
                    };
                    db.BACSIs.Add(newBS);
                }
                else // Sửa
                {
                    var bs = db.BACSIs.Find(editingBacSi.MaBacSi);
                    if (bs != null)
                    {
                        bs.HoTen = hoTen;
                        bs.SoDienThoai = soDienThoai;
                        bs.Email = email;
                        bs.ChuyenKhoa = chuyenKhoa;

                        if (selectedAvatarBytes != null)
                            bs.AnhDaiDien = selectedAvatarBytes;
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

        private void Button_ChonAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (dlg.ShowDialog() == true)
            {
                string filePath = dlg.FileName;

                // Hiển thị ảnh trong Image control
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                imgAvatar.Source = bitmap;

                // Lưu file thành byte[]
                selectedAvatarBytes = File.ReadAllBytes(filePath);
            }
        }
    }
}

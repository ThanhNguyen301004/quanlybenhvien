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
using System.IO;
using Microsoft.Win32;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for Form_TaiKhoan.xaml
    /// </summary>
    public partial class Form_TaiKhoan : Window
    {
        QLBVEntities db = new QLBVEntities();
        private byte[] selectedAvatarBytes;
        private TAIKHOAN editingTaiKhoan;

        public Form_TaiKhoan()
        {
            InitializeComponent();
        }

        public Form_TaiKhoan(TAIKHOAN tk) : this()
        {
            editingTaiKhoan = tk;

            Tb_matKhau.Text = tk.MaKhau;

            ComboBox_loaiTaiKhoan.Text = tk.LoaiTK;

            if (tk.AnhCaNhan != null)
            {
                using (var ms = new MemoryStream(tk.AnhCaNhan))
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
                string matKhau = Tb_matKhau.Text.Trim();
                string loaiTaiKhoan = (ComboBox_loaiTaiKhoan.SelectedItem as ComboBoxItem)?.Content.ToString();

                if (editingTaiKhoan == null) // Thêm mới
                {
                    TAIKHOAN newTK = new TAIKHOAN
                    {
                        MaKhau = matKhau,
                        LoaiTK = loaiTaiKhoan,
                        AnhCaNhan = selectedAvatarBytes
                    };
                    db.TAIKHOANs.Add(newTK);
                }
                else // Sửa
                {
                    var bs = db.TAIKHOANs.Find(editingTaiKhoan.TaiKhoan1);
                    if (bs != null)
                    {
                        bs.MaKhau = matKhau;
                        bs.LoaiTK = loaiTaiKhoan;

                        if (selectedAvatarBytes != null)
                            bs.AnhCaNhan = selectedAvatarBytes;
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

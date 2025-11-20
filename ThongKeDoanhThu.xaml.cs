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
    /// <summary>
    /// Interaction logic for ThongKeDoanhThu.xaml
    /// </summary>
    public partial class ThongKeDoanhThu : Page
    {
        QLBVEntities db = new QLBVEntities();

        public ThongKeDoanhThu()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            DG_HOADONVIENPHI.ItemsSource = db.HOADONVIENPHIs.ToList();
            
        }

        private void Button_ThongKe_Click(object sender, RoutedEventArgs e)
        {
            if (dp_tuNgay.SelectedDate == null || dp_denNgay.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn khoảng ngày đầy đủ!");
                return;
            }

            DateTime tuNgay = dp_tuNgay.SelectedDate.Value.Date;
            DateTime denNgay = dp_denNgay.SelectedDate.Value.Date;

            using (var db = new QLBVEntities())
            {
                var hoaDonList = db.HOADONVIENPHIs
                    .Include("BENHNHAN")
                    .Include("PHONGBENH")
                    .Where(hd => hd.NgayXuatVien >= tuNgay && hd.NgayXuatVien <= denNgay)
                    .ToList();
                
                int tongSoCa = hoaDonList.Count;
                
                decimal tongDoanhThu = hoaDonList.Sum(hd => (decimal?)hd.ThanhTien) ?? 0;
                
                Lb_soCa.Content = tongSoCa.ToString();
                Lb_tongDoanhThu.Content = $"{tongDoanhThu:N0} VND";

                DG_HOADONVIENPHI.ItemsSource = hoaDonList;
            }
        }

        private void Button_LamMoi_Click(object sender, RoutedEventArgs e)
        {
            dp_tuNgay.SelectedDate = DateTime.Now;
            dp_denNgay.SelectedDate = DateTime.Now;

            Lb_soCa.Content = "0";
            Lb_tongDoanhThu.Content = "0 VND";

            DG_HOADONVIENPHI.ItemsSource = null;
        }


    }
}

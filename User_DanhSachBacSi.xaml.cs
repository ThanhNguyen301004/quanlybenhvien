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
using System.Collections.ObjectModel;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for User_DanhSachBacSi.xaml
    /// </summary>
    public partial class User_DanhSachBacSi : Page
    {
        QLBVEntities db = new QLBVEntities();
        private ObservableCollection<BACSI> allDoctors;
        private ObservableCollection<BACSI> filteredDoctors;
        private string currentSpecialty = "";
        private string currentSearchText = "";
        private Button activeFilterButton;

        public User_DanhSachBacSi()
        {
            InitializeComponent();
            InitializeCollections();
            activeFilterButton = BtnTatCa;
            LoadData();
        }

        private void InitializeCollections()
        {
            allDoctors = new ObservableCollection<BACSI>();
            filteredDoctors = new ObservableCollection<BACSI>();
            DoctorsItemsControl.ItemsSource = filteredDoctors;

        }

        private void UpdateFilteredList()
        {
            try
            {
                
                // Kiểm tra null
                if (allDoctors == null || filteredDoctors == null)
                    return;

                filteredDoctors.Clear();
            
                var query = allDoctors.Where(d => d != null); // Lọc null items

                // Lọc theo chuyên khoa
                if (!string.IsNullOrEmpty(currentSpecialty))
                {
                    query = query.Where(d => d.ChuyenKhoa != null &&
                                            d.ChuyenKhoa.Contains(currentSpecialty));
                }

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrWhiteSpace(currentSearchText))
                {
                    query = query.Where(d =>
                        (d.HoTen != null && d.HoTen.ToLower().Contains(currentSearchText.ToLower())) ||
                        (d.ChuyenKhoa != null && d.ChuyenKhoa.ToLower().Contains(currentSearchText.ToLower())) ||
                        (d.Email != null && d.Email.ToLower().Contains(currentSearchText.ToLower())));
                }


                // Thêm kết quả
                foreach (var doctor in query)
                {
                    filteredDoctors.Add(doctor);
                }

                UpdateResultsCount();
                UpdateUIState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc danh sách: {ex.Message}", "Lỗi");
            }
        }

        private void LoadData()
        {
            try
            {
                // Hiển thị loading
                ShowLoadingState();

                // Xóa dữ liệu cũ
                allDoctors.Clear();
                filteredDoctors.Clear();

                // Load dữ liệu từ database
                var doctors = db.BACSIs.ToList();

                // Thêm vào collection
                foreach (var doctor in doctors)
                {
                    allDoctors.Add(doctor);
                }

                UpdateFilteredList();

                // Ẩn loading
                HideLoadingState();

                // Cập nhật số lượng kết quả
                UpdateResultsCount();
            }
            catch (Exception ex)
            {
                // Ẩn loading nếu có lỗi
                HideLoadingState();

                // Hiển thị thông báo lỗi
                MessageBox.Show($"Lỗi khi tải danh sách bác sĩ: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                // Hiển thị trạng thái không có kết quả
                ShowNoResultsState();
            }
        }

        

        private void UpdateResultsCount()
        {
            int count = filteredDoctors.Count;
            if (count == 0)
            {
                ResultsCountText.Text = "Không tìm thấy bác sĩ nào";
            }
            else if (count == 1)
            {
                ResultsCountText.Text = "Tìm thấy 1 bác sĩ";
            }
            else
            {
                ResultsCountText.Text = $"Tìm thấy {count} bác sĩ";
            }
        }

        private void UpdateUIState()
        {
            if (filteredDoctors.Count == 0)
            {
                ShowNoResultsState();
            }
            else
            {
                ShowNormalState();
            }
        }

        private void ShowLoadingState()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            NoResultsPanel.Visibility = Visibility.Collapsed;
            DoctorsItemsControl.Visibility = Visibility.Collapsed;
        }

        private void HideLoadingState()
        {
            LoadingPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowNoResultsState()
        {
            NoResultsPanel.Visibility = Visibility.Visible;
            DoctorsItemsControl.Visibility = Visibility.Collapsed;
        }

        private void ShowNormalState()
        {
            NoResultsPanel.Visibility = Visibility.Collapsed;
            DoctorsItemsControl.Visibility = Visibility.Visible;
        }

        // Event Handlers cho Search
        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == "Tìm kiếm bác sĩ theo tên..." && textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = "";
                textBox.Foreground = Brushes.Black;
                textBox.FontStyle = FontStyles.Normal;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Tìm kiếm bác sĩ theo tên...";
                textBox.Foreground = Brushes.Gray;
                textBox.FontStyle = FontStyles.Italic;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearchText = SearchTextBox.Text;

            // Nếu vẫn là placeholder thì coi như rỗng
            if (currentSearchText == "Tìm kiếm bác sĩ theo tên...")
            {
                currentSearchText = string.Empty;
            }

            UpdateFilteredList();
        }


        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Foreground != Brushes.Gray)
            {
                currentSearchText = SearchTextBox.Text.Trim();
                UpdateFilteredList();
            }
        }

        // Event Handler cho Filter buttons
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            // Cập nhật trạng thái active button
            if (activeFilterButton != null)
            {
                activeFilterButton.Style = (Style)FindResource("FilterButtonStyle");
            }

            activeFilterButton = clickedButton;
            activeFilterButton.Style = (Style)FindResource("ActiveFilterButtonStyle");

            // Cập nhật filter
            currentSpecialty = clickedButton.Tag.ToString();
            UpdateFilteredList();
        }

        // Event Handlers cho Doctor actions
        private void DoctorCard_Click(object sender, MouseButtonEventArgs e)
        {
            Border card = sender as Border;

            if (card.DataContext is BACSI doctor)
            {
                ViewDoctorDetails(doctor);
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.CommandParameter is BACSI doctor)
            {
                ViewDoctorDetails(doctor);
            }
        }

        private void BookAppointment_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.CommandParameter is BACSI doctor)
            {
                User_DatLichHen dlh = new User_DatLichHen(doctor);

                bool? result = dlh.ShowDialog();
            }
        }

        private void ViewDoctorDetails(BACSI doctor)
        {
            try
            {
                // Tạo cửa sổ chi tiết bác sĩ
                string details = $"Thông tin chi tiết:\n\n" +
                                $"Họ tên: {doctor.HoTen}\n" +
                                $"Chuyên khoa: {doctor.ChuyenKhoa}\n" +
                                $"Số điện thoại: {doctor.SoDienThoai ?? "Chưa cập nhật"}\n" +
                                $"Email: {doctor.Email ?? "Chưa cập nhật"}\n" +
                                $"Mã bác sĩ: BS{doctor.MaBacSi:D4}";

                MessageBox.Show(details, $"Chi tiết - {doctor.HoTen}",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị chi tiết: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BookAppointmentWithDoctor(BACSI doctor)
        {
            try
            {
                // Hiển thị dialog xác nhận đặt lịch
                var result = MessageBox.Show(
                    $"Bạn có muốn đặt lịch khám với {doctor.HoTen} - {doctor.ChuyenKhoa}?",
                    "Xác nhận đặt lịch",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Trong thực tế, sẽ mở form đặt lịch hoặc navigate đến trang đặt lịch
                    // Ví dụ: NavigationService.Navigate(new DatLichHenPage(doctor));

                    MessageBox.Show(
                        $"Đặt lịch thành công với {doctor.HoTen}!\nVui lòng chờ xác nhận từ bệnh viện.",
                        "Thành công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lịch: {ex.Message}",
                               "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm refresh data (có thể gọi từ bên ngoài)
        public void RefreshData()
        {
            LoadData();
        }
        

    }
}

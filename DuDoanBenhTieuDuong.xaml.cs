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
using System.Net.Http;

namespace QUANLYBENHVIEN
{
    /// <summary>
    /// Interaction logic for KHACH.xaml
    /// </summary>
    public partial class DuDoanBenhTieuDuong : Page
    {
        public DuDoanBenhTieuDuong()
        {
            InitializeComponent();
        }

        private async void BtnPredict_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double pregnancies = double.Parse(txtPregnancies.Text);
                double glucose = double.Parse(txtGlucose.Text);
                double bloodPressure = double.Parse(txtBloodPressure.Text);
                double skinThickness = double.Parse(txtSkinThickness.Text);
                double insulin = double.Parse(txtInsulin.Text);
                double bmi = double.Parse(txtBMI.Text);
                double dpf = double.Parse(txtDPF.Text);
                double age = double.Parse(txtAge.Text);

                string result = await PredictDiabetesAsync(
                    pregnancies, glucose, bloodPressure,
                    skinThickness, insulin, bmi, dpf, age);

                txtResult.Text = "Kết quả: " + result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập liệu: " + ex.Message);
            }
        }

        private async Task<string> PredictDiabetesAsync(
        double pregnancies, double glucose, double bloodPressure,
        double skinThickness, double insulin, double bmi,
        double dpf, double age)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
            {
                { "Pregnancies", pregnancies.ToString() },
                { "Glucose", glucose.ToString() },
                { "BloodPressure", bloodPressure.ToString() },
                { "SkinThickness", skinThickness.ToString() },
                { "Insulin", insulin.ToString() },
                { "BMI", bmi.ToString() },
                { "DiabetesPedigreeFunction", dpf.ToString() },
                { "Age", age.ToString() }
            };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://127.0.0.1:5000/predict", content);

                // Đọc JSON trả về
                var json = await response.Content.ReadAsStringAsync();

                // Parse JSON để lấy "prediction"
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                return data.prediction;
            }
        }
    }
}

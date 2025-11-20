using System.Net;
using System.Net.Mail;
using System;
using System.Windows;

namespace QUANLYBENHVIEN
{
    public class EmailHelper
    {
        public static void SendEmail(string to, string subject, string body)
        {
            try
            {
                var fromAddress = new MailAddress("nguyenvanthanh301004@gmail.com", "Phòng khám");
                var toAddress = new MailAddress(to);

                string fromPassword = "orvn ruma ellf bawr";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Không thể gửi email: " + ex.Message);
            }
        }
    }
}

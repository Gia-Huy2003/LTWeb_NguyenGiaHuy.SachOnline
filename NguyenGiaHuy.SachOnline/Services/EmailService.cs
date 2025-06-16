using System.Net;
using System.Net.Mail;

namespace NguyenGiaHuy.SachOnline.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "nguyengiahuy732003@gmail.com";
        private readonly string _fromPassword = "tdtsbrgihlwoampj";

        public void SendOrderConfirmationEmail(string toEmail, string subject, string body)
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_fromEmail, _fromPassword),
                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}

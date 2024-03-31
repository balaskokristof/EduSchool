using System.Net;
using System.Net.Mail;

namespace EduSchool.Models.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = "kristof@kopajler.hu";
            var password = "Monita20042011.";

            var client = new SmtpClient("smtp.forpsi.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true,
            };

            return client.SendMailAsync(
                               new MailMessage(mail, email, subject, htmlMessage) { IsBodyHtml = true });

        }
    }
}

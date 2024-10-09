using StudentInfoSystemApp.Application.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        public void SendEmail(List<string> emails, string subject, string body)
        {
            MailMessage mailMessage = new();
            mailMessage.From = new MailAddress("emilnm@code.edu.az", "StudentInfoSystem");

            foreach (var email in emails)
                mailMessage.To.Add(email);

            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;

            SmtpClient smtpClient = new();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("emilnm@code.edu.az", "yexu wlwj egqm axpw");
            smtpClient.Send(mailMessage);
        }
    }
}

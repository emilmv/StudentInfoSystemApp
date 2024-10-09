using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StudentInfoSystemApp.Application.Services.Interfaces;
using StudentInfoSystemApp.Core.Entities;
using System.Net;
using System.Net.Mail;

namespace StudentInfoSystemApp.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmailService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

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
        public async Task<string> GenerateEmailConfirmationLinkAsync(ApplicationUser User)
        {
            // Generate the email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(User);

            // Get the scheme and host from the HTTP context
            var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
            var host = _httpContextAccessor.HttpContext.Request.Host.Value;

            // Create the confirmation link
            return $"{scheme}://{host}/api/Auth/verify-email?email={User.Email}&token={Uri.EscapeDataString(token)}";
        }
        public async Task<string> GenerateVerificationEmailBodyAsync(string confirmationLink, string username)
        {
            // Read the email template
            string body = string.Empty;
            using (StreamReader streamReader = new StreamReader("wwwroot/templates/emailVerificationTemplate.html"))
            {
                body = await streamReader.ReadToEndAsync();
            }

            // Replace placeholders with actual values
            body = body.Replace("{{link}}", confirmationLink);
            body = body.Replace("{{username}}", username);

            return body;
        }
        public async Task<string> GenerateResetPasswordLinkAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var scheme = _httpContextAccessor.HttpContext.Request.Scheme;

            var host = _httpContextAccessor.HttpContext.Request.Host.Value;

            return $"{scheme}://{host}/api/auth/reset-password?email={user.Email}&token={token}";
        }

        public string GenerateResetPasswordEmailBody(string resetLink, string userName)
        {
            string body = string.Empty;
            using (StreamReader streamReader = new StreamReader("wwwroot/templates/resetPasswordTemplate.html"))
            {
                body = streamReader.ReadToEnd();
            }

            body = body.Replace("{{username}}", userName);
            body = body.Replace("{{resetLink}}", resetLink);
            return body;
        }
    }
}

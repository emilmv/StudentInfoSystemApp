using StudentInfoSystemApp.Core.Entities;

namespace StudentInfoSystemApp.Application.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(List<string> emails, string subject, string body);
        Task<string> GenerateEmailConfirmationLinkAsync(ApplicationUser user);
        Task<string> GenerateVerificationEmailBodyAsync(string confirmationLink, string username);
        Task<string> GenerateResetPasswordLinkAsync(ApplicationUser user);
        string GenerateResetPasswordEmailBody(string confirmationLink, string username);
    }
}

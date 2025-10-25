using CorisSeguros.Notification.Models;

namespace CorisSeguros.Notification.Services;

public interface IEmailService
{
    Task<EmailResult> SendEmailAsync(string to, string subject, string body);
}




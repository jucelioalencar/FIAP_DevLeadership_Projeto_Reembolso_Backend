using Azure.Communication.Email;
using Azure.Core;
using CorisSeguros.Notification.Models;

namespace CorisSeguros.Notification.Services;

public class AzureEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly string _fromEmail;
    private readonly ILogger<AzureEmailService> _logger;

    public AzureEmailService(
        IConfiguration configuration,
        ILogger<AzureEmailService> logger)
    {
        var connectionString = configuration.GetConnectionString("AzureCommunication");
        _emailClient = new EmailClient(connectionString);
        _fromEmail = configuration["EmailSettings:FromEmail"] ?? "noreply@corisseguros.com";
        _logger = logger;
    }

    public async Task<EmailResult> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Enviando e-mail para {To} com assunto: {Subject}", to, subject);

            var emailContent = new EmailContent(subject)
            {
                PlainText = body,
                Html = $"<html><body><p>{body.Replace("\n", "<br>")}</p></body></html>"
            };

            var emailMessage = new EmailMessage(
                senderAddress: _fromEmail,
                recipientAddress: to,
                content: emailContent);

            var emailOperation = await _emailClient.SendAsync(
                Azure.WaitUntil.Completed,
                emailMessage);

            if (emailOperation.HasCompleted)
            {
                _logger.LogInformation("E-mail enviado com sucesso para {To}", to);
                return new EmailResult
                {
                    Success = true,
                    MessageId = emailOperation.Id
                };
            }
            else
            {
                _logger.LogWarning("Falha ao enviar e-mail para {To}", to);
                return new EmailResult
                {
                    Success = false,
                    ErrorMessage = "Falha na operação de envio"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail para {To}", to);
            return new EmailResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}




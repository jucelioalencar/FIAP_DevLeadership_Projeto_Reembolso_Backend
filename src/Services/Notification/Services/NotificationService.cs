using CorisSeguros.Notification.Models;
using CorisSeguros.Notification.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CorisSeguros.Notification.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        NotificationDbContext context,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _context = context;
        _logger = logger;
    }

    public async Task<NotificationResult> SendNotificationAsync(NotificationRecord notification)
    {
        try
        {
            _logger.LogInformation("Enviando notificação {NotificationId} para {Recipient}", 
                notification.Id, notification.Recipient);

            // Salvar notificação no banco
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Enviar notificação baseada no tipo de destinatário
            NotificationResult result;

            switch (notification.RecipientType)
            {
                case RecipientType.Analyst:
                    result = await SendAnalystNotificationAsync(notification);
                    break;
                    
                case RecipientType.Passenger:
                    result = await SendPassengerNotificationAsync(notification);
                    break;
                    
                case RecipientType.Manager:
                    result = await SendManagerNotificationAsync(notification);
                    break;
                    
                default:
                    result = new NotificationResult
                    {
                        NotificationId = notification.Id,
                        Status = NotificationStatus.Failed,
                        Message = "Tipo de destinatário não suportado",
                        SentAt = DateTime.UtcNow
                    };
                    break;
            }

            // Atualizar status da notificação
            notification.Status = result.Status;
            notification.SentAt = result.SentAt;
            notification.ErrorMessage = result.Message;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Notificação {NotificationId} processada com status: {Status}", 
                notification.Id, result.Status);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação {NotificationId}", notification.Id);
            
            notification.Status = NotificationStatus.Failed;
            notification.ErrorMessage = ex.Message;
            await _context.SaveChangesAsync();

            return new NotificationResult
            {
                NotificationId = notification.Id,
                Status = NotificationStatus.Failed,
                Message = ex.Message,
                SentAt = DateTime.UtcNow
            };
        }
    }

    public async Task<List<NotificationRecord>> GetNotificationsByDocumentAsync(Guid documentId)
    {
        return await _context.Notifications
            .Where(n => n.DocumentId == documentId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    private async Task<NotificationResult> SendAnalystNotificationAsync(NotificationRecord notification)
    {
        // Para analistas, enviar notificação interna (dashboard)
        // Em um cenário real, isso seria uma notificação push ou websocket
        
        _logger.LogInformation("Notificação interna para analista: {Subject}", notification.Subject);
        
        return new NotificationResult
        {
            NotificationId = notification.Id,
            Status = NotificationStatus.Sent,
            Message = "Notificação interna enviada",
            SentAt = DateTime.UtcNow
        };
    }

    private async Task<NotificationResult> SendPassengerNotificationAsync(NotificationRecord notification)
    {
        try
        {
            // Enviar e-mail para o passageiro
            var emailResult = await _emailService.SendEmailAsync(
                notification.Recipient,
                notification.Subject,
                notification.Message);

            if (emailResult.Success)
            {
                return new NotificationResult
                {
                    NotificationId = notification.Id,
                    Status = NotificationStatus.Sent,
                    Message = "E-mail enviado com sucesso",
                    SentAt = DateTime.UtcNow
                };
            }
            else
            {
                return new NotificationResult
                {
                    NotificationId = notification.Id,
                    Status = NotificationStatus.Failed,
                    Message = emailResult.ErrorMessage,
                    SentAt = DateTime.UtcNow
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar e-mail para {Recipient}", notification.Recipient);
            return new NotificationResult
            {
                NotificationId = notification.Id,
                Status = NotificationStatus.Failed,
                Message = ex.Message,
                SentAt = DateTime.UtcNow
            };
        }
    }

    private async Task<NotificationResult> SendManagerNotificationAsync(NotificationRecord notification)
    {
        // Para gerentes, enviar notificação interna (dashboard)
        _logger.LogInformation("Notificação interna para gerente: {Subject}", notification.Subject);
        
        return new NotificationResult
        {
            NotificationId = notification.Id,
            Status = NotificationStatus.Sent,
            Message = "Notificação interna enviada",
            SentAt = DateTime.UtcNow
        };
    }
}




using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Notification.Services;
using CorisSeguros.Notification.Models;
using CorisSeguros.Notification.DTOs;

namespace CorisSeguros.Notification.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(
        INotificationService notificationService,
        ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequestDto request)
    {
        try
        {
            _logger.LogInformation("Enviando notificação para {RecipientType}: {Recipient}", 
                request.RecipientType, request.Recipient);

            var notification = new NotificationRecord
            {
                Id = Guid.NewGuid(),
                DocumentId = request.DocumentId,
                RecipientType = request.RecipientType,
                Recipient = request.Recipient,
                Subject = request.Subject,
                Message = request.Message,
                NotificationType = request.NotificationType,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _notificationService.SendNotificationAsync(notification);

            _logger.LogInformation("Notificação {NotificationId} enviada com status: {Status}", 
                notification.Id, result.Status);

            return Ok(new NotificationResponseDto
            {
                NotificationId = result.NotificationId,
                Status = result.Status.ToString(),
                Message = result.Message,
                SentAt = result.SentAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("history/{documentId}")]
    public async Task<IActionResult> GetNotificationHistory(Guid documentId)
    {
        try
        {
            var notifications = await _notificationService.GetNotificationsByDocumentAsync(documentId);
            
            var response = notifications.Select(n => new NotificationHistoryDto
            {
                Id = n.Id,
                RecipientType = n.RecipientType.ToString(),
                Recipient = n.Recipient,
                Subject = n.Subject,
                Status = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar histórico de notificações para documento {DocumentId}", documentId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("approve/{documentId}")]
    public async Task<IActionResult> NotifyApproval(Guid documentId, [FromBody] ApprovalNotificationDto approval)
    {
        try
        {
            _logger.LogInformation("Enviando notificação de aprovação para documento {DocumentId}", documentId);

            var notification = new NotificationRecord
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                RecipientType = RecipientType.Passenger,
                Recipient = approval.PassengerEmail,
                Subject = "Reembolso Aprovado - Coris Seguros",
                Message = $"Seu pedido de reembolso foi aprovado. Valor: R$ {approval.Amount:F2}. {approval.Notes}",
                NotificationType = NotificationType.Approval,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _notificationService.SendNotificationAsync(notification);

            return Ok(new NotificationResponseDto
            {
                NotificationId = result.NotificationId,
                Status = result.Status.ToString(),
                Message = result.Message,
                SentAt = result.SentAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de aprovação para documento {DocumentId}", documentId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("reject/{documentId}")]
    public async Task<IActionResult> NotifyRejection(Guid documentId, [FromBody] RejectionNotificationDto rejection)
    {
        try
        {
            _logger.LogInformation("Enviando notificação de rejeição para documento {DocumentId}", documentId);

            var notification = new NotificationRecord
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                RecipientType = RecipientType.Passenger,
                Recipient = rejection.PassengerEmail,
                Subject = "Reembolso Não Aprovado - Coris Seguros",
                Message = $"Seu pedido de reembolso não foi aprovado. Motivo: {rejection.Reason}",
                NotificationType = NotificationType.Rejection,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _notificationService.SendNotificationAsync(notification);

            return Ok(new NotificationResponseDto
            {
                NotificationId = result.NotificationId,
                Status = result.Status.ToString(),
                Message = result.Message,
                SentAt = result.SentAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de rejeição para documento {DocumentId}", documentId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}




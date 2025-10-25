using CorisSeguros.Notification.Models;

namespace CorisSeguros.Notification.DTOs;

public class NotificationRequestDto
{
    public Guid DocumentId { get; set; }
    public RecipientType RecipientType { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; }
}

public class NotificationResponseDto
{
    public Guid NotificationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class NotificationHistoryDto
{
    public Guid Id { get; set; }
    public string RecipientType { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

public class ApprovalNotificationDto
{
    public string PassengerEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
}

public class RejectionNotificationDto
{
    public string PassengerEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}




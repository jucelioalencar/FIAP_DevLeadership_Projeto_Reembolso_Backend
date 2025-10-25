namespace CorisSeguros.Notification.Models;

public class NotificationResult
{
    public Guid NotificationId { get; set; }
    public NotificationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}




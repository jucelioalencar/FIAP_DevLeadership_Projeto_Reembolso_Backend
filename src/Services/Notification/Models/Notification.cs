using System.ComponentModel.DataAnnotations;

namespace CorisSeguros.Notification.Models;

public class NotificationRecord
{
    [Key]
    public Guid Id { get; set; }
    
    public Guid DocumentId { get; set; }
    
    public RecipientType RecipientType { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Recipient { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public NotificationType NotificationType { get; set; }
    
    public NotificationStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? SentAt { get; set; }
    
    public string? ErrorMessage { get; set; }
}

public enum RecipientType
{
    Analyst,
    Passenger,
    Manager
}

public enum NotificationType
{
    DocumentUploaded,
    ProcessingStarted,
    ProcessingCompleted,
    Approval,
    Rejection,
    Error
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Retrying
}




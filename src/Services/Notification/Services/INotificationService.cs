using CorisSeguros.Notification.Models;

namespace CorisSeguros.Notification.Services;

public interface INotificationService
{
    Task<NotificationResult> SendNotificationAsync(NotificationRecord notification);
    Task<List<NotificationRecord>> GetNotificationsByDocumentAsync(Guid documentId);
}




namespace CorisSeguros.Notification.Services;

public interface IServiceBusService
{
    Task SendMessageAsync<T>(string queueName, T message);
    Task SendMessageAsync(string queueName, string message);
}




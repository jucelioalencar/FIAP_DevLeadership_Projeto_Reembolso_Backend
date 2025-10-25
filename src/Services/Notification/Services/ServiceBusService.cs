using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace CorisSeguros.Notification.Services;

public class ServiceBusService : IServiceBusService
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<ServiceBusService> _logger;

    public ServiceBusService(
        IConfiguration configuration,
        ILogger<ServiceBusService> logger)
    {
        var connectionString = configuration.GetConnectionString("ServiceBus");
        _serviceBusClient = new ServiceBusClient(connectionString);
        _logger = logger;
    }

    public async Task SendMessageAsync<T>(string queueName, T message)
    {
        try
        {
            var sender = _serviceBusClient.CreateSender(queueName);
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(serviceBusMessage);
            _logger.LogInformation("Mensagem enviada para a fila {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para a fila {QueueName}", queueName);
            throw;
        }
    }

    public async Task SendMessageAsync(string queueName, string message)
    {
        try
        {
            var sender = _serviceBusClient.CreateSender(queueName);
            var serviceBusMessage = new ServiceBusMessage(message)
            {
                ContentType = "text/plain"
            };

            await sender.SendMessageAsync(serviceBusMessage);
            _logger.LogInformation("Mensagem enviada para a fila {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para a fila {QueueName}", queueName);
            throw;
        }
    }
}




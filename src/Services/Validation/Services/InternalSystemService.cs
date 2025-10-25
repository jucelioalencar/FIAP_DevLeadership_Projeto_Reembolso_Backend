using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.Services;

public class InternalSystemService : IInternalSystemService
{
    private readonly ILogger<InternalSystemService> _logger;

    public InternalSystemService(ILogger<InternalSystemService> logger)
    {
        _logger = logger;
    }

    public async Task<InternalSystemResult> ValidatePassengerAsync(string passengerName, string flightNumber)
    {
        try
        {
            _logger.LogInformation("Validando passageiro {PassengerName} no voo {FlightNumber} com sistemas internos", 
                passengerName, flightNumber);

            // Simulação de validação com sistemas internos (I-syst, Tavola)
            // Em um cenário real, aqui você faria chamadas para APIs internas
            
            await Task.Delay(100); // Simular latência de rede

            // Mock de validação - em produção, isso seria uma chamada real para os sistemas
            var isValid = await ValidateWithISystAsync(passengerName, flightNumber);
            var tavolaResult = await ValidateWithTavolaAsync(passengerName, flightNumber);

            var result = new InternalSystemResult
            {
                IsValid = isValid && tavolaResult.IsValid,
                PassengerName = passengerName,
                FlightNumber = flightNumber,
                ISystValidation = new ISystResult
                {
                    IsValid = isValid,
                    PassengerFound = isValid,
                    LastChecked = DateTime.UtcNow
                },
                TavolaValidation = tavolaResult,
                ValidationTimestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Validação interna concluída para {PassengerName}. Válido: {IsValid}", 
                passengerName, result.IsValid);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar passageiro {PassengerName} com sistemas internos", passengerName);
            return new InternalSystemResult
            {
                IsValid = false,
                PassengerName = passengerName,
                FlightNumber = flightNumber,
                ErrorMessage = ex.Message,
                ValidationTimestamp = DateTime.UtcNow
            };
        }
    }

    private async Task<bool> ValidateWithISystAsync(string passengerName, string flightNumber)
    {
        // Simulação de validação com I-syst
        await Task.Delay(50);
        
        // Mock: assumir que o passageiro existe se o nome não estiver vazio
        return !string.IsNullOrWhiteSpace(passengerName);
    }

    private async Task<TavolaResult> ValidateWithTavolaAsync(string passengerName, string flightNumber)
    {
        // Simulação de validação com Tavola
        await Task.Delay(50);
        
        return new TavolaResult
        {
            IsValid = !string.IsNullOrWhiteSpace(passengerName),
            PassengerFound = !string.IsNullOrWhiteSpace(passengerName),
            LastChecked = DateTime.UtcNow
        };
    }
}




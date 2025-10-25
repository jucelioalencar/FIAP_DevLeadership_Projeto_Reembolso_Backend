using System.Text.Json;
using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.Services;

public class FlightAwareValidationService : IFlightValidationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FlightAwareValidationService> _logger;

    public FlightAwareValidationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<FlightAwareValidationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<FlightAwareResult> ValidateFlightAsync(string flightNumber, DateTime? flightDate)
    {
        try
        {
            _logger.LogInformation("Validando voo {FlightNumber} com FlightAware", flightNumber);

            var apiKey = _configuration["ExternalApis:FlightAware:ApiKey"];
            var baseUrl = _configuration["ExternalApis:FlightAware:BaseUrl"];

            // Construir URL da API
            var requestUrl = $"{baseUrl}/flights/{flightNumber}";
            if (flightDate.HasValue)
            {
                var dateString = flightDate.Value.ToString("yyyy-MM-dd");
                requestUrl += $"?date={dateString}";
            }

            // Adicionar API key
            requestUrl += requestUrl.Contains('?') ? "&" : "?";
            requestUrl += $"api_key={apiKey}";

            _logger.LogDebug("Fazendo requisição para: {RequestUrl}", requestUrl);

            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var flightData = JsonSerializer.Deserialize<FlightAwareResponse>(content);

                if (flightData?.Flights?.Any() == true)
                {
                    var flight = flightData.Flights.First();
                    
                    _logger.LogInformation("Voo {FlightNumber} encontrado no FlightAware", flightNumber);

                    return new FlightAwareResult
                    {
                        IsValid = true,
                        FlightNumber = flightNumber,
                        Airline = flight.Airline,
                        Origin = flight.Origin,
                        Destination = flight.Destination,
                        ScheduledDeparture = flight.ScheduledDeparture,
                        ActualDeparture = flight.ActualDeparture,
                        Status = flight.Status,
                        DelayMinutes = flight.DelayMinutes,
                        Confidence = 0.9
                    };
                }
                else
                {
                    _logger.LogWarning("Voo {FlightNumber} não encontrado no FlightAware", flightNumber);
                    return new FlightAwareResult
                    {
                        IsValid = false,
                        FlightNumber = flightNumber,
                        ErrorMessage = "Voo não encontrado",
                        Confidence = 0.1
                    };
                }
            }
            else
            {
                _logger.LogWarning("Erro na API FlightAware: {StatusCode}", response.StatusCode);
                return new FlightAwareResult
                {
                    IsValid = false,
                    FlightNumber = flightNumber,
                    ErrorMessage = $"Erro na API: {response.StatusCode}",
                    Confidence = 0.0
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao validar voo {FlightNumber} com FlightAware", flightNumber);
            return new FlightAwareResult
            {
                IsValid = false,
                FlightNumber = flightNumber,
                ErrorMessage = ex.Message,
                Confidence = 0.0
            };
        }
    }

    public async Task<FlightStatus> GetFlightStatusAsync(string flightNumber)
    {
        try
        {
            var validationResult = await ValidateFlightAsync(flightNumber, null);
            
            return new FlightStatus
            {
                FlightNumber = flightNumber,
                IsValid = validationResult.IsValid,
                Status = validationResult.Status,
                DelayMinutes = validationResult.DelayMinutes,
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do voo {FlightNumber}", flightNumber);
            return new FlightStatus
            {
                FlightNumber = flightNumber,
                IsValid = false,
                Status = "Unknown",
                LastUpdated = DateTime.UtcNow
            };
        }
    }
}




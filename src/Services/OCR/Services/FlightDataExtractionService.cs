using System.Text.RegularExpressions;
using CorisSeguros.OCR.Models;

namespace CorisSeguros.OCR.Services;

public class FlightDataExtractionService : IDataExtractionService
{
    private readonly ILogger<FlightDataExtractionService> _logger;

    public FlightDataExtractionService(ILogger<FlightDataExtractionService> logger)
    {
        _logger = logger;
    }

    public async Task<FlightData> ExtractFlightDataAsync(string ocrText)
    {
        try
        {
            _logger.LogInformation("Extraindo dados de voo do texto OCR");

            var flightData = new FlightData();

            // Extrair número do voo (padrões comuns: AA1234, G31234, LA1234, etc.)
            var flightNumberPattern = @"\b([A-Z]{2,3})\s*(\d{3,4})\b";
            var flightMatch = Regex.Match(ocrText, flightNumberPattern, RegexOptions.IgnoreCase);
            if (flightMatch.Success)
            {
                flightData.FlightNumber = $"{flightMatch.Groups[1].Value}{flightMatch.Groups[2].Value}";
                _logger.LogInformation("Número do voo extraído: {FlightNumber}", flightData.FlightNumber);
            }

            // Extrair nome do passageiro
            var passengerNamePattern = @"(?:PASSAGEIRO|PASSENGER|NOME|NAME)[\s:]*([A-Z\s]+)";
            var passengerMatch = Regex.Match(ocrText, passengerNamePattern, RegexOptions.IgnoreCase);
            if (passengerMatch.Success)
            {
                flightData.PassengerName = passengerMatch.Groups[1].Value.Trim();
                _logger.LogInformation("Nome do passageiro extraído: {PassengerName}", flightData.PassengerName);
            }

            // Extrair data do voo
            var datePattern = @"(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{2,4})";
            var dateMatch = Regex.Match(ocrText, datePattern);
            if (dateMatch.Success)
            {
                var day = int.Parse(dateMatch.Groups[1].Value);
                var month = int.Parse(dateMatch.Groups[2].Value);
                var year = int.Parse(dateMatch.Groups[3].Value);
                
                if (year < 100) year += 2000; // Assumir século 21 para anos de 2 dígitos
                
                try
                {
                    flightData.FlightDate = new DateTime(year, month, day);
                    _logger.LogInformation("Data do voo extraída: {FlightDate}", flightData.FlightDate);
                }
                catch (ArgumentException)
                {
                    _logger.LogWarning("Data inválida extraída: {Day}/{Month}/{Year}", day, month, year);
                }
            }

            // Extrair horário de partida
            var timePattern = @"(\d{1,2}):(\d{2})";
            var timeMatches = Regex.Matches(ocrText, timePattern);
            if (timeMatches.Count > 0)
            {
                var firstTime = timeMatches[0];
                var hour = int.Parse(firstTime.Groups[1].Value);
                var minute = int.Parse(firstTime.Groups[2].Value);
                
                flightData.ScheduledDeparture = new TimeSpan(hour, minute, 0);
                _logger.LogInformation("Horário de partida extraído: {ScheduledDeparture}", flightData.ScheduledDeparture);
            }

            // Extrair aeroportos (códigos IATA)
            var airportPattern = @"\b([A-Z]{3})\b";
            var airportMatches = Regex.Matches(ocrText, airportPattern);
            var airports = airportMatches.Cast<Match>()
                .Select(m => m.Value)
                .Where(code => IsValidAirportCode(code))
                .Distinct()
                .ToList();

            if (airports.Count >= 2)
            {
                flightData.Origin = airports[0];
                flightData.Destination = airports[1];
                _logger.LogInformation("Aeroportos extraídos: {Origin} -> {Destination}", 
                    flightData.Origin, flightData.Destination);
            }

            // Extrair valor do bilhete
            var pricePattern = @"(?:R\$|USD|EUR)\s*([\d,\.]+)";
            var priceMatch = Regex.Match(ocrText, pricePattern, RegexOptions.IgnoreCase);
            if (priceMatch.Success)
            {
                var priceText = priceMatch.Groups[1].Value.Replace(",", ".");
                if (decimal.TryParse(priceText, out var price))
                {
                    flightData.TicketPrice = price;
                    _logger.LogInformation("Valor do bilhete extraído: {TicketPrice}", flightData.TicketPrice);
                }
            }

            flightData.ExtractionTimestamp = DateTime.UtcNow;
            flightData.Confidence = CalculateExtractionConfidence(flightData);

            _logger.LogInformation("Extração de dados concluída com confiança: {Confidence}%", 
                flightData.Confidence);

            return flightData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante a extração de dados do voo");
            throw;
        }
    }

    private static bool IsValidAirportCode(string code)
    {
        // Lista de códigos IATA comuns (simplificada)
        var validCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GRU", "CGH", "BSB", "SDU", "GIG", "CNF", "REC", "SSA", "FOR", "BEL",
            "MIA", "JFK", "LAX", "ORD", "DFW", "ATL", "LHR", "CDG", "FRA", "MAD",
            "EZE", "SCL", "LIM", "BOG", "PTY", "MEX", "YYZ", "YVR"
        };
        
        return validCodes.Contains(code);
    }

    private static double CalculateExtractionConfidence(FlightData flightData)
    {
        var confidence = 0.0;
        
        if (!string.IsNullOrEmpty(flightData.FlightNumber)) confidence += 25;
        if (!string.IsNullOrEmpty(flightData.PassengerName)) confidence += 25;
        if (flightData.FlightDate != default) confidence += 20;
        if (flightData.ScheduledDeparture != default) confidence += 15;
        if (!string.IsNullOrEmpty(flightData.Origin) && !string.IsNullOrEmpty(flightData.Destination)) confidence += 15;
        
        return confidence;
    }
}


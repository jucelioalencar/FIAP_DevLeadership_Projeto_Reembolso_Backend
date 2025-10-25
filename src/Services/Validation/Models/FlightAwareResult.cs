namespace CorisSeguros.Validation.Models;

public class FlightAwareResult
{
    public bool IsValid { get; set; }
    public string FlightNumber { get; set; } = string.Empty;
    public string? Airline { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? ScheduledDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public string? Status { get; set; }
    public int? DelayMinutes { get; set; }
    public double Confidence { get; set; }
    public string? ErrorMessage { get; set; }
}

public class FlightAwareResponse
{
    public List<FlightAwareFlight>? Flights { get; set; }
}

public class FlightAwareFlight
{
    public string? Airline { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? ScheduledDeparture { get; set; }
    public DateTime? ActualDeparture { get; set; }
    public string? Status { get; set; }
    public int? DelayMinutes { get; set; }
}




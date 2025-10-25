namespace CorisSeguros.Validation.Models;

public class FlightStatus
{
    public string FlightNumber { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? DelayMinutes { get; set; }
    public DateTime LastUpdated { get; set; }
}




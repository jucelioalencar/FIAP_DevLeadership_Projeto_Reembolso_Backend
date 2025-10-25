namespace CorisSeguros.Analysis.Models;

public class ValidationResult
{
    public string FlightNumber { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public double Confidence { get; set; }
    public FlightAwareValidation? FlightAwareValidation { get; set; }
    public InternalSystemValidation? InternalSystemValidation { get; set; }
    public DateTime ValidationTimestamp { get; set; }
}

public class FlightAwareValidation
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

public class InternalSystemValidation
{
    public bool IsValid { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public ISystValidation? ISystValidation { get; set; }
    public TavolaValidation? TavolaValidation { get; set; }
    public DateTime ValidationTimestamp { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ISystValidation
{
    public bool IsValid { get; set; }
    public bool PassengerFound { get; set; }
    public DateTime LastChecked { get; set; }
}

public class TavolaValidation
{
    public bool IsValid { get; set; }
    public bool PassengerFound { get; set; }
    public DateTime LastChecked { get; set; }
}




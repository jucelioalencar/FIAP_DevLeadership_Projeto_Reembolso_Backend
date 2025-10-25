namespace CorisSeguros.Validation.Models;

public class InternalSystemResult
{
    public bool IsValid { get; set; }
    public string PassengerName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public ISystResult? ISystValidation { get; set; }
    public TavolaResult? TavolaValidation { get; set; }
    public DateTime ValidationTimestamp { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ISystResult
{
    public bool IsValid { get; set; }
    public bool PassengerFound { get; set; }
    public DateTime LastChecked { get; set; }
}

public class TavolaResult
{
    public bool IsValid { get; set; }
    public bool PassengerFound { get; set; }
    public DateTime LastChecked { get; set; }
}




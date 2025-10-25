namespace CorisSeguros.Validation.Models;

public class FlightValidationResult
{
    public string FlightNumber { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public double Confidence { get; set; }
    public FlightAwareResult? FlightAwareValidation { get; set; }
    public InternalSystemResult? InternalSystemValidation { get; set; }
    public DateTime ValidationTimestamp { get; set; }
}




using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.DTOs;

public class FlightValidationRequestDto
{
    public string FlightNumber { get; set; } = string.Empty;
    public string? PassengerName { get; set; }
    public DateTime? FlightDate { get; set; }
}

public class FlightValidationResponseDto
{
    public bool IsValid { get; set; }
    public double Confidence { get; set; }
    public FlightAwareResult? FlightAwareResult { get; set; }
    public InternalSystemResult? InternalSystemResult { get; set; }
    public DateTime ValidationTimestamp { get; set; }
}




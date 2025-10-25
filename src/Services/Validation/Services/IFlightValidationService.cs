using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.Services;

public interface IFlightValidationService
{
    Task<FlightAwareResult> ValidateFlightAsync(string flightNumber, DateTime? flightDate);
    Task<FlightStatus> GetFlightStatusAsync(string flightNumber);
}




using CorisSeguros.Validation.Models;

namespace CorisSeguros.Validation.Services;

public interface IInternalSystemService
{
    Task<InternalSystemResult> ValidatePassengerAsync(string passengerName, string flightNumber);
}




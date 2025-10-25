using CorisSeguros.OCR.Models;

namespace CorisSeguros.OCR.Services;

public interface IDatabaseService
{
    Task UpdateDocumentStatusAsync(Guid documentId, string status);
    Task SaveExtractedDataAsync(Guid documentId, FlightData flightData);
    Task SendToValidationAsync(Guid documentId, FlightData flightData);
}




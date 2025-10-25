using CorisSeguros.OCR.Models;

namespace CorisSeguros.OCR.Services;

public interface IDataExtractionService
{
    Task<FlightData> ExtractFlightDataAsync(string ocrText);
}


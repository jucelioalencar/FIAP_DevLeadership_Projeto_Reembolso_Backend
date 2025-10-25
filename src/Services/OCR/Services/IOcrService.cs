using CorisSeguros.OCR.Models;

namespace CorisSeguros.OCR.Services;

public interface IOcrService
{
    Task<OcrResult> ExtractTextAsync(string blobUrl);
}


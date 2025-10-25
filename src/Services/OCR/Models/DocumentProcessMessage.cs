namespace CorisSeguros.OCR.Models;

public class DocumentProcessMessage
{
    public Guid DocumentId { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? PassengerName { get; set; }
    public string? FlightNumber { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}


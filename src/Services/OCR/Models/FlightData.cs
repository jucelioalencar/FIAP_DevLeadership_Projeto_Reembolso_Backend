namespace CorisSeguros.OCR.Models;

public class FlightData
{
    public string? FlightNumber { get; set; }
    public string? PassengerName { get; set; }
    public DateTime? FlightDate { get; set; }
    public TimeSpan? ScheduledDeparture { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public decimal? TicketPrice { get; set; }
    public DateTime ExtractionTimestamp { get; set; }
    public double Confidence { get; set; }
    public string? RawOcrText { get; set; }
}




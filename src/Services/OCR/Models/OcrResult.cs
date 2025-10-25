namespace CorisSeguros.OCR.Models;

public class OcrResult
{
    public bool Success { get; set; }
    public string Text { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}


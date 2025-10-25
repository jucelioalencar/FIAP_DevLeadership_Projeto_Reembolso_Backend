using Azure;
using Azure.AI.Vision.ImageAnalysis;
using Azure.Storage.Blobs;
using CorisSeguros.OCR.Models;

namespace CorisSeguros.OCR.Services;

public class AzureVisionOcrService : IOcrService
{
    private readonly ImageAnalysisClient _imageAnalysisClient;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureVisionOcrService> _logger;

    public AzureVisionOcrService(
        IConfiguration configuration,
        ILogger<AzureVisionOcrService> logger)
    {
        var endpoint = configuration["AzureVision:Endpoint"];
        var key = configuration["AzureVision:Key"];
        
        _imageAnalysisClient = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
        _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureStorage"));
        _logger = logger;
    }

    public async Task<OcrResult> ExtractTextAsync(string blobUrl)
    {
        try
        {
            _logger.LogInformation("Iniciando OCR para {BlobUrl}", blobUrl);

            // Baixar imagem do blob storage
            var blobClient = new BlobClient(new Uri(blobUrl));
            using var imageStream = await blobClient.OpenReadAsync();

            // Configurar opções de análise
            var options = new ImageAnalysisOptions
            {
                Features = ImageAnalysisFeature.Text,
                Language = "pt-BR,en-US", // Português e Inglês
                GenderNeutralCaption = false
            };

            // Executar análise
            var result = await _imageAnalysisClient.AnalyzeAsync(
                BinaryData.FromStream(imageStream), 
                options);

            if (result.Value.Text != null && result.Value.Text.Blocks.Any())
            {
                var extractedText = string.Join("\n", 
                    result.Value.Text.Blocks.Select(block => 
                        string.Join(" ", block.Lines.Select(line => line.Content))));

                _logger.LogInformation("OCR concluído com sucesso. Texto extraído: {Length} caracteres", 
                    extractedText.Length);

                return new OcrResult
                {
                    Success = true,
                    Text = extractedText,
                    Confidence = CalculateAverageConfidence(result.Value.Text.Blocks)
                };
            }
            else
            {
                _logger.LogWarning("Nenhum texto foi extraído do documento");
                return new OcrResult
                {
                    Success = false,
                    ErrorMessage = "Nenhum texto foi detectado no documento"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o OCR para {BlobUrl}", blobUrl);
            return new OcrResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static double CalculateAverageConfidence(IEnumerable<TextBlock> blocks)
    {
        var confidences = blocks
            .SelectMany(block => block.Lines)
            .SelectMany(line => line.Words)
            .Where(word => word.Confidence.HasValue)
            .Select(word => word.Confidence!.Value);

        return confidences.Any() ? confidences.Average() : 0.0;
    }
}


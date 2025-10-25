using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CorisSeguros.OCR.Models;
using CorisSeguros.OCR.Services;
using System.Text.Json;

namespace CorisSeguros.OCR.Functions;

public class DocumentProcessingFunction
{
    private readonly IOcrService _ocrService;
    private readonly IDataExtractionService _dataExtractionService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<DocumentProcessingFunction> _logger;

    public DocumentProcessingFunction(
        IOcrService ocrService,
        IDataExtractionService dataExtractionService,
        IDatabaseService databaseService,
        ILogger<DocumentProcessingFunction> logger)
    {
        _ocrService = ocrService;
        _dataExtractionService = dataExtractionService;
        _databaseService = databaseService;
        _logger = logger;
    }

    [Function("DocumentProcessing")]
    public async Task Run(
        [ServiceBusTrigger("document-processing", Connection = "ServiceBusConnection")] string message)
    {
        try
        {
            _logger.LogInformation("Processando documento: {Message}", message);

            var processMessage = JsonSerializer.Deserialize<DocumentProcessMessage>(message);
            if (processMessage == null)
            {
                _logger.LogError("Mensagem inválida recebida");
                return;
            }

            // Atualizar status para processando
            await _databaseService.UpdateDocumentStatusAsync(processMessage.DocumentId, "Processing");

            // Fazer OCR do documento
            var ocrResult = await _ocrService.ExtractTextAsync(processMessage.BlobUrl);
            
            if (!ocrResult.Success)
            {
                _logger.LogError("Falha no OCR para documento {DocumentId}: {Error}", 
                    processMessage.DocumentId, ocrResult.ErrorMessage);
                await _databaseService.UpdateDocumentStatusAsync(processMessage.DocumentId, "Error");
                return;
            }

            // Extrair dados estruturados do texto OCR
            var extractedData = await _dataExtractionService.ExtractFlightDataAsync(ocrResult.Text);
            
            // Salvar dados extraídos no banco
            await _databaseService.SaveExtractedDataAsync(processMessage.DocumentId, extractedData);
            
            // Atualizar status para OCR concluído
            await _databaseService.UpdateDocumentStatusAsync(processMessage.DocumentId, "OCRCompleted");

            _logger.LogInformation("OCR concluído para documento {DocumentId}", processMessage.DocumentId);

            // Enviar para próxima etapa (Validação)
            await _databaseService.SendToValidationAsync(processMessage.DocumentId, extractedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar documento");
            throw;
        }
    }
}


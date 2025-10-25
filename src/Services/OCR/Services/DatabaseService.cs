using System.Text.Json;
using CorisSeguros.OCR.Models;
using CorisSeguros.OCR.Infrastructure;

namespace CorisSeguros.OCR.Services;

public class DatabaseService : IDatabaseService
{
    private readonly OcrDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(OcrDbContext context, ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task UpdateDocumentStatusAsync(Guid documentId, string status)
    {
        try
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document != null)
            {
                document.Status = status;
                document.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Status do documento {DocumentId} atualizado para {Status}", 
                    documentId, status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do documento {DocumentId}", documentId);
            throw;
        }
    }

    public async Task SaveExtractedDataAsync(Guid documentId, FlightData flightData)
    {
        try
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document != null)
            {
                document.ExtractedData = JsonSerializer.Serialize(flightData);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Dados extraídos salvos para documento {DocumentId}", documentId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar dados extraídos para documento {DocumentId}", documentId);
            throw;
        }
    }

    public async Task SendToValidationAsync(Guid documentId, FlightData flightData)
    {
        try
        {
            // Aqui você implementaria o envio para o serviço de validação
            // Por enquanto, apenas logamos a ação
            _logger.LogInformation("Enviando documento {DocumentId} para validação", documentId);
            
            // TODO: Implementar envio para Service Bus ou chamada HTTP para serviço de validação
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar documento {DocumentId} para validação", documentId);
            throw;
        }
    }
}




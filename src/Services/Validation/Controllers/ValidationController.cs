using Microsoft.AspNetCore.Mvc;
using CorisSeguros.Validation.Services;
using CorisSeguros.Validation.Models;
using CorisSeguros.Validation.DTOs;

namespace CorisSeguros.Validation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValidationController : ControllerBase
{
    private readonly IFlightValidationService _flightValidationService;
    private readonly IInternalSystemService _internalSystemService;
    private readonly ILogger<ValidationController> _logger;

    public ValidationController(
        IFlightValidationService flightValidationService,
        IInternalSystemService internalSystemService,
        ILogger<ValidationController> logger)
    {
        _flightValidationService = flightValidationService;
        _internalSystemService = internalSystemService;
        _logger = logger;
    }

    [HttpPost("validate-flight")]
    public async Task<IActionResult> ValidateFlight([FromBody] FlightValidationRequestDto request)
    {
        try
        {
            _logger.LogInformation("Iniciando validação do voo {FlightNumber}", request.FlightNumber);

            var validationResult = new FlightValidationResult
            {
                FlightNumber = request.FlightNumber,
                ValidationTimestamp = DateTime.UtcNow
            };

            // Validar com FlightAware
            var flightAwareResult = await _flightValidationService.ValidateFlightAsync(
                request.FlightNumber, 
                request.FlightDate);

            validationResult.FlightAwareValidation = flightAwareResult;

            // Validar com sistemas internos
            var internalValidation = await _internalSystemService.ValidatePassengerAsync(
                request.PassengerName, 
                request.FlightNumber);

            validationResult.InternalSystemValidation = internalValidation;

            // Determinar resultado final
            validationResult.IsValid = flightAwareResult.IsValid && internalValidation.IsValid;
            validationResult.Confidence = CalculateValidationConfidence(validationResult);

            _logger.LogInformation("Validação concluída para voo {FlightNumber}. Válido: {IsValid}", 
                request.FlightNumber, validationResult.IsValid);

            return Ok(new FlightValidationResponseDto
            {
                IsValid = validationResult.IsValid,
                Confidence = validationResult.Confidence,
                FlightAwareResult = flightAwareResult,
                InternalSystemResult = internalValidation,
                ValidationTimestamp = validationResult.ValidationTimestamp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação do voo {FlightNumber}", request.FlightNumber);
            return StatusCode(500, "Erro interno do servidor durante validação");
        }
    }

    [HttpGet("flight-status/{flightNumber}")]
    public async Task<IActionResult> GetFlightStatus(string flightNumber)
    {
        try
        {
            var status = await _flightValidationService.GetFlightStatusAsync(flightNumber);
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar status do voo {FlightNumber}", flightNumber);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    private static double CalculateValidationConfidence(FlightValidationResult result)
    {
        var confidence = 0.0;

        if (result.FlightAwareValidation?.IsValid == true)
            confidence += 60;

        if (result.InternalSystemValidation?.IsValid == true)
            confidence += 40;

        return confidence;
    }
}




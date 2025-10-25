using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CorisSeguros.OCR.Services;
using CorisSeguros.OCR.Infrastructure;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddScoped<IOcrService, AzureVisionOcrService>();
        services.AddScoped<IDataExtractionService, FlightDataExtractionService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
    })
    .Build();

host.Run();


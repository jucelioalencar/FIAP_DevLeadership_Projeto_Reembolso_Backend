using CorisSeguros.Validation.Services;
using CorisSeguros.Validation.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do banco de dados
builder.Services.AddDbContext<ValidationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de serviços
builder.Services.AddScoped<IFlightValidationService, FlightAwareValidationService>();
builder.Services.AddScoped<IInternalSystemService, InternalSystemService>();
builder.Services.AddScoped<IServiceBusService, ServiceBusService>();

// Configuração de HTTP Client para APIs externas
builder.Services.AddHttpClient<IFlightValidationService, FlightAwareValidationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ExternalApis:FlightAware:BaseUrl"] ?? "");
    client.DefaultRequestHeaders.Add("User-Agent", "CorisSeguros-Validation/1.0");
});

// Configuração de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => "Validation service is healthy");

app.Run();




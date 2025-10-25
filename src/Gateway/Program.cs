using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuração do YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

// Configuração de autenticação (Azure AD)
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["AzureAd:Authority"];
        options.Audience = builder.Configuration["AzureAd:Audience"];
        options.RequireHttpsMetadata = false; // Apenas para desenvolvimento
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware de CORS
app.UseCors("AllowAll");

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Configuração do proxy reverso
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => "Gateway is healthy");

app.Run();


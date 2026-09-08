using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PetHealthAPI.Infraestrutura.Health;
using System.Text.Json;
using PetHealthAPI.Aplicacao.Middlewares;
using Serilog;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using PetHealthAPI.Infraestrutura.Observabilidade;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/app-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pet Health API",
        Version = "v1",
        Description = "API Restful para a plataforma Pet Health — cuidado contínuo e preventivo para pets. " +
                      "Gerencie tutores, pets, vacinas, consultas e medicamentos de forma organizada.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Pet Health — Challenge FIAP 2026",
            Email = "rm556649@fiap.com.br"
        }
    });
});

builder.Services.AddHealthChecks()
    .AddOracle(
        builder.Configuration.GetConnectionString("OracleConnection")!,
        name: "oracle-database",
        tags: new[] { "db", "oracle" })
    .AddCheck<ServicoExternoHealthCheck>(
        "servico-externo",
        tags: new[] { "external" });

builder.Services.AddSingleton<AplicacaoMetricas>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource(AplicacaoMetricas.NomeActivitySource)
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddMeter("PetHealthAPI.API")
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pet Health API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var resposta = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                nome = e.Key,
                status = e.Value.Status.ToString(),
                descricao = e.Value.Description,
                duracaoMs = e.Value.Duration.TotalMilliseconds,
                dados = e.Value.Data
            }),
            duracaoTotalMs = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(resposta, new JsonSerializerOptions { WriteIndented = true }));
    }
});

app.Run();

public partial class Program { }

using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pet Health API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();

app.Run();

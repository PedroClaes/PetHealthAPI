using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PetHealthAPI.Infraestrutura.Health
{
    public class ServicoExternoHealthCheck : IHealthCheck
    {
        private static readonly Random _random = new();

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // Simula disponibilidade de um serviço externo (ex: gateway de notificação)
            var disponivel = _random.Next(1, 10) > 1; // ~90% de sucesso
            var dados = new Dictionary<string, object>
            {
                { "LatenciaMs", _random.Next(5, 50) }
            };

            if (disponivel)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Serviço externo disponível.", dados));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("Serviço externo indisponível no momento.", data: dados));
        }
    }
}
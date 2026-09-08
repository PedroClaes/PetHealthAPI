using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace PetHealthAPI.Infraestrutura.Observabilidade
{
    public class AplicacaoMetricas
    {
        public const string NomeActivitySource = "PetHealthAPI.API";

        public static readonly ActivitySource ActivitySource = new(NomeActivitySource, "1.0.0");

        private readonly Meter _meter;
        private readonly Counter<long> _petsCriados;

        public AplicacaoMetricas()
        {
            _meter = new Meter("PetHealthAPI.API", "1.0.0");
            _petsCriados = _meter.CreateCounter<long>(
                name: "pets_criados_total",
                unit: "{pets}",
                description: "Total de pets criados, com tag de status (sucesso ou erro_validacao).");
        }

        public void RegistrarPetCriado(string status)
        {
            _petsCriados.Add(1, new KeyValuePair<string, object?>("status", status));
        }
    }
}
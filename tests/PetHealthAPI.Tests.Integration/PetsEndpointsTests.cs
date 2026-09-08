using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PetHealthAPI.Tests.Integration
{
    [Collection("Integration Tests")]
    public class PetsEndpointsTests
    {
        private readonly HttpClient _client;

        public PetsEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetHealth_QuandoChamado_RetornaOk()
        {
            // Arrange
            // (nenhuma preparação adicional necessária)

            // Act
            var resposta = await _client.GetAsync("/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task PostPets_ComDadosValidos_RetornaCreated()
        {
            // Arrange
            var novoPet = new
            {
                nome = "TesteIntegracao",
                especie = "Cao",
                raca = "Vira-lata",
                idade = 2,
                peso = 10,
                sexo = "Macho",
                castrado = true,
                dataNascimento = "2024-01-01T00:00:00Z",
                tutorId = 1
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/pets", novoPet);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostPets_ComNomeVazio_RetornaBadRequest()
        {
            // Arrange
            var petInvalido = new
            {
                nome = "",
                especie = "Cao",
                raca = "Vira-lata",
                idade = 2,
                peso = 10,
                sexo = "Macho",
                castrado = true,
                dataNascimento = "2024-01-01T00:00:00Z",
                tutorId = 1
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/pets", petInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }
    }
}
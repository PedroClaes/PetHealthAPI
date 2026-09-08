using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace PetHealthAPI.Tests.Integration
{
    [Collection("Integration Tests")]
    public class TutoresEndpointsTests
    {
        private readonly HttpClient _client;

        public TutoresEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task PostTutores_ComDadosValidos_RetornaCreated()
        {
            // Arrange
            var novoTutor = new
            {
                nome = "TutorTeste",
                email = $"tutor{Guid.NewGuid():N}@teste.com",
                telefone = "11999999999",
                endereco = "Rua Teste, 123"
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/tutores", novoTutor);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task PostTutores_ComEmailInvalido_RetornaBadRequest()
        {
            // Arrange
            var tutorInvalido = new
            {
                nome = "TutorInvalido",
                email = "nao-e-um-email",
                telefone = "11999999999",
                endereco = "Rua Teste, 123"
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/tutores", tutorInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }
    }
}
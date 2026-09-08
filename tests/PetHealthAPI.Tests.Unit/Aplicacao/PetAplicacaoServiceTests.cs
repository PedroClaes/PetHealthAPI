using FluentAssertions;
using Moq;
using PetHealthAPI.Aplicacao.Servicos;
using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Models;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Aplicacao
{
    public class PetAplicacaoServiceTests
    {
        private static Pet CriarPetValido()
        {
            return new Pet
            {
                Nome = "Rex",
                Especie = "Cão",
                Idade = 3,
                Peso = 15,
                TutorId = 1
            };
        }

        [Fact]
        public async Task PrepararCadastroAsync_TutorExistenteEDadosValidos_NaoLancaExcecao()
        {
            // Arrange
            var pet = CriarPetValido();
            var repositorioMock = new Mock<ITutorRepositorio>();
            repositorioMock.Setup(r => r.ExisteAsync(pet.TutorId)).ReturnsAsync(true);
            var servico = new PetAplicacaoService(repositorioMock.Object);

            // Act
            var acao = async () => await servico.PrepararCadastroAsync(pet);

            // Assert
            await acao.Should().NotThrowAsync();
            repositorioMock.Verify(r => r.ExisteAsync(pet.TutorId), Times.Once);
        }

        [Fact]
        public async Task PrepararCadastroAsync_TutorInexistente_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            var repositorioMock = new Mock<ITutorRepositorio>();
            repositorioMock.Setup(r => r.ExisteAsync(pet.TutorId)).ReturnsAsync(false);
            var servico = new PetAplicacaoService(repositorioMock.Object);

            // Act
            var acao = async () => await servico.PrepararCadastroAsync(pet);

            // Assert
            await acao.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"Tutor com ID {pet.TutorId} não encontrado. Cadastre o tutor antes do pet.");
        }

        [Fact]
        public async Task PrepararCadastroAsync_NomeInvalido_LancaArgumentException_SemConsultarRepositorio()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Nome = "";
            var repositorioMock = new Mock<ITutorRepositorio>();
            var servico = new PetAplicacaoService(repositorioMock.Object);

            // Act
            var acao = async () => await servico.PrepararCadastroAsync(pet);

            // Assert
            await acao.Should().ThrowAsync<ArgumentException>()
                .WithMessage("O nome do pet é obrigatório.");
            repositorioMock.Verify(r => r.ExisteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
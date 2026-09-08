using FluentAssertions;
using PetHealthAPI.Dominio.Validacoes;
using PetHealthAPI.Models;
using Xunit;

namespace PetHealthAPI.Tests.Unit.Dominio
{
    public class PetValidadorTests
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
        public void Validar_PetComDadosValidos_NaoLancaExcecao()
        {
            // Arrange
            var pet = CriarPetValido();

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().NotThrow();
        }

        [Fact]
        public void Validar_NomeVazio_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Nome = "";

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("O nome do pet é obrigatório.");
        }

        [Fact]
        public void Validar_NomeComEspacosEmBranco_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Nome = "   ";

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("O nome do pet é obrigatório.");
        }

        [Fact]
        public void Validar_IdadeNegativa_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Idade = -1;

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("A idade do pet não pode ser negativa.");
        }

        [Fact]
        public void Validar_PesoZero_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Peso = 0;

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("O peso do pet deve ser maior que zero.");
        }

        [Fact]
        public void Validar_PesoNegativo_LancaArgumentException()
        {
            // Arrange
            var pet = CriarPetValido();
            pet.Peso = -5;

            // Act
            var acao = () => PetValidador.Validar(pet);

            // Assert
            acao.Should().Throw<ArgumentException>()
                .WithMessage("O peso do pet deve ser maior que zero.");
        }
    }
}
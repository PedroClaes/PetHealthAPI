using PetHealthAPI.Dominio.Interfaces;
using PetHealthAPI.Dominio.Validacoes;
using PetHealthAPI.Models;

namespace PetHealthAPI.Aplicacao.Servicos
{
    public class PetAplicacaoService
    {
        private readonly ITutorRepositorio _tutorRepositorio;

        public PetAplicacaoService(ITutorRepositorio tutorRepositorio)
        {
            _tutorRepositorio = tutorRepositorio;
        }

        public async Task PrepararCadastroAsync(Pet pet)
        {
            PetValidador.Validar(pet);

            var tutorExiste = await _tutorRepositorio.ExisteAsync(pet.TutorId);
            if (!tutorExiste)
                throw new ArgumentException($"Tutor com ID {pet.TutorId} não encontrado. Cadastre o tutor antes do pet.");
        }
    }
}
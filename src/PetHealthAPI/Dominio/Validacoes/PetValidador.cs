using PetHealthAPI.Models;

namespace PetHealthAPI.Dominio.Validacoes
{
    public static class PetValidador
    {
        public static void Validar(Pet pet)
        {
            if (string.IsNullOrWhiteSpace(pet.Nome))
                throw new ArgumentException("O nome do pet é obrigatório.");

            if (pet.Idade < 0)
                throw new ArgumentException("A idade do pet não pode ser negativa.");

            if (pet.Peso <= 0)
                throw new ArgumentException("O peso do pet deve ser maior que zero.");
        }
    }
}
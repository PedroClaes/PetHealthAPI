namespace PetHealthAPI.Dominio.Interfaces
{
    public interface ITutorRepositorio
    {
        Task<bool> ExisteAsync(int tutorId);
    }
}
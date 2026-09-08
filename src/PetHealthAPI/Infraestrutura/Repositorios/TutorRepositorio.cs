using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Dominio.Interfaces;

namespace PetHealthAPI.Infraestrutura.Repositorios
{
    public class TutorRepositorio : ITutorRepositorio
    {
        private readonly AppDbContext _context;

        public TutorRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(int tutorId)
        {
            return await _context.Tutores.AnyAsync(t => t.Id == tutorId);
        }
    }
}
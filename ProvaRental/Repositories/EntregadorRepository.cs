using Microsoft.EntityFrameworkCore;
using System;

namespace ProvaRental.Repositories
{
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly ProvaContext _context;

        public EntregadorRepository(ProvaContext context)
        {
            _context = context;
        }

        public async Task<Entregador> GetByIdAsync(int id)
        {
            return await _context.Entregadores.FindAsync(id);
        }

        public async Task<IEnumerable<Entregador>> GetAllAsync()
        {
            return await _context.Entregadores.ToListAsync();
        }

        public async Task AddAsync(Entregador entregador)
        {
            _context.Entregadores.Add(entregador);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Entregador entregador)
        {
            _context.Entregadores.Update(entregador);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entregador = await GetByIdAsync(id);
            if (entregador != null)
            {
                _context.Entregadores.Remove(entregador);
                await _context.SaveChangesAsync();
            }
        }
    }

}

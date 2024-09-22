using Microsoft.EntityFrameworkCore;
using System;

namespace ProvaRental.Repositories
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly ProvaContext _dbContext;

        public LocacaoRepository(ProvaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Locacao locacao)
        {
            await _dbContext.Locacoes.AddAsync(locacao);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Locacao> GetByIdAsync(int id)
        {
            return await _dbContext.Locacoes.FindAsync(id);
        }

        public async Task<IEnumerable<Locacao>> GetAllAsync()
        {
            return await _dbContext.Locacoes.ToListAsync();
        }

        public async Task UpdateAsync(Locacao locacao)
        {
            _dbContext.Locacoes.Update(locacao);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<bool> ExisteLocacaoPorMotoIdAsync(int motoId)
        {
            return await _dbContext.Locacoes.AnyAsync(locacao => locacao.MotoId == motoId);
        }
    }
}


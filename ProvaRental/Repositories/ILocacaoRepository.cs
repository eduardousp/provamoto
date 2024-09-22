namespace ProvaRental.Repositories
{
    public interface ILocacaoRepository
    {
        Task AddAsync(Locacao locacao);
        Task<Locacao> GetByIdAsync(int id);
        Task<IEnumerable<Locacao>> GetAllAsync();
        Task UpdateAsync(Locacao locacao);
        Task<bool> ExisteLocacaoPorMotoIdAsync(int motoId);
    }
}

namespace ProvaRental.Repositories
{
    public interface IEntregadorRepository
    {
        Task<Entregador> GetByIdAsync(int id);
        Task<IEnumerable<Entregador>> GetAllAsync();
        Task AddAsync(Entregador entregador);
        Task UpdateAsync(Entregador entregador);
        Task DeleteAsync(int id);
    }

}

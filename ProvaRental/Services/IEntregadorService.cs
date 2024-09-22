namespace ProvaRental.Services
{
    public interface IEntregadorService
    {
        Task<Entregador> GetEntregadorByIdAsync(int id);
        Task<IEnumerable<Entregador>> GetAllEntregadoresAsync();
        Task AddEntregadorAsync(Entregador entregador);
        Task UpdateEntregadorAsync(Entregador entregador);
        Task DeleteEntregadorAsync(int id);
    }

}

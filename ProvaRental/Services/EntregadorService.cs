using ProvaRental.Repositories;

namespace ProvaRental.Services
{
    public class EntregadorService:IEntregadorService
    {
        private readonly IEntregadorRepository _repository;

        public EntregadorService(IEntregadorRepository repository)
        {
            _repository = repository;
        }

        public Task<Entregador> GetEntregadorByIdAsync(int id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Entregador>> GetAllEntregadoresAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task AddEntregadorAsync(Entregador entregador)
        {
            return _repository.AddAsync(entregador);
        }

        public Task UpdateEntregadorAsync(Entregador entregador)
        {
            return _repository.UpdateAsync(entregador);
        }

        public Task DeleteEntregadorAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }

}

namespace ProvaRental.Services
{
    public interface ILocacaoService
    {
        Task<Locacao> CriarLocacaoAsync(Locacao locacao);
        Task<Locacao> GetLocacaoByIdAsync(int id);
        Task<IEnumerable<Locacao>> GetAllLocacoesAsync();
        Task AtualizarLocacaoAsync(Locacao locacao);
        decimal CalcularValorTotal(Locacao locacao);
    }
}

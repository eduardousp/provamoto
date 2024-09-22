using ProvaRental.Repositories;

namespace ProvaRental.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _locacaoRepository;

        public LocacaoService(ILocacaoRepository locacaoRepository)
        {
            _locacaoRepository = locacaoRepository;
        }

        public async Task<Locacao> CriarLocacaoAsync(Locacao locacao)
        {
            await _locacaoRepository.AddAsync(locacao);
            return locacao;
        }

        public async Task<Locacao> GetLocacaoByIdAsync(int id)
        {
            return await _locacaoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Locacao>> GetAllLocacoesAsync()
        {
            return await _locacaoRepository.GetAllAsync();
        }

        public async Task AtualizarLocacaoAsync(Locacao locacao)
        {
            await _locacaoRepository.UpdateAsync(locacao);
        }

        public decimal CalcularValorTotal(Locacao locacao)
        {
            var diasTotais = (locacao.DataDevolucao - locacao.DataInicio)?.Days ?? 0;
            var diasRestantes = (locacao.DataPrevisaoTermino - locacao.DataDevolucao)?.Days ?? 0;

            decimal valorPorDia = locacao.Plano switch
            {
                PlanoLocacao.SeteDias => 30m,
                PlanoLocacao.QuinzeDias => 28m,
                PlanoLocacao.TrintaDias => 22m,
                PlanoLocacao.QuarentaCincoDias => 20m,
                PlanoLocacao.CinquentaDias => 18m,
                _ => throw new InvalidOperationException("Plano inválido")
            };

            decimal valorTotal = diasTotais * valorPorDia;

            // Aplicar multa ou cobrança adicional
            if (diasRestantes > 0)
            {
                decimal multa = locacao.Plano switch
                {
                    PlanoLocacao.SeteDias => 0.20m,
                    PlanoLocacao.QuinzeDias => 0.40m,
                    _ => 0m
                };
                valorTotal += diasRestantes * valorPorDia * multa;
            }
            else if (diasRestantes < 0)
            {
                valorTotal += Math.Abs(diasRestantes) * 50m;
            }

            return valorTotal;
        }
    }
}

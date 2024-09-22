using ProvaRental.Repositories;

public class MotoService : IMotoService
{
    private readonly IMotoRepository _motoRepository;
    private readonly ILocacaoRepository _locacaoRepository;
    private readonly IRabbitMQPublisher _publisher;

    public MotoService(IMotoRepository motoRepository, IRabbitMQPublisher publisher, ILocacaoRepository locacaoRepository)
    {
        _motoRepository = motoRepository;
        _publisher = publisher;
        _locacaoRepository = locacaoRepository;
    }

    public async Task<Moto> CadastrarMoto(Moto moto)
    {
        // Verificar se a placa já existe
        var motoExistente = await _motoRepository.GetMotosAsync();
        if (motoExistente.Any(m => m.Placa == moto.Placa))
            throw new Exception("Já existe uma moto cadastrada com essa placa.");

        await _motoRepository.AddMotoAsync(moto);

        // Publicar evento no RabbitMQ
        _publisher.PublishMotoCadastrada(moto);

        return moto;
    }

    public async Task<IEnumerable<Moto>> ConsultarMotosAsync()
    {
        return await _motoRepository.GetMotosAsync();
    }

    public async Task<Moto> ConsultarMotoPorIdAsync(int id)
    {
        var moto = await _motoRepository.GetMotoByIdAsync(id);
        if (moto == null)
            throw new Exception("Moto não encontrada.");

        return moto;
    }

    public async Task<Moto> AlterarPlacaAsync(int id, string novaPlaca)
    {
        var moto = await _motoRepository.GetMotoByIdAsync(id);
        if (moto == null)
            throw new Exception("Moto não encontrada.");

        // Atualizar a placa
        moto.Placa = novaPlaca;
        await _motoRepository.UpdateMotoAsync(moto);

        return moto;
    }

    public async Task<bool> RemoverMotoAsync(int id)
    {
        var moto = await _motoRepository.GetMotoByIdAsync(id);
        if (moto == null)
            throw new Exception("Moto não encontrada.");

        var locacoesAssociadas = await _locacaoRepository.ExisteLocacaoPorMotoIdAsync(id);

        if (locacoesAssociadas)
        {
            // Não é possível remover a moto, pois há locações associadas
            return false;
        }
        await _motoRepository.DeleteMotoAsync(id);
        return true;
    }
}

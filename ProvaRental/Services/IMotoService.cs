public interface IMotoService
{
    Task<Moto> CadastrarMoto(Moto moto);
    Task<IEnumerable<Moto>> ConsultarMotosAsync();
    Task<Moto> ConsultarMotoPorIdAsync(int id);
    Task<Moto> AlterarPlacaAsync(int id, string novaPlaca);
    Task<bool> RemoverMotoAsync(int id);
}

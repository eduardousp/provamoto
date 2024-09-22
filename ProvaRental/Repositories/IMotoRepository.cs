public interface IMotoRepository
{
    Task<IEnumerable<Moto>> GetMotosAsync();
    Task<Moto> GetMotoByIdAsync(int id);
    Task AddMotoAsync(Moto moto);
    Task UpdateMotoAsync(Moto moto);
    Task DeleteMotoAsync(int id);
}

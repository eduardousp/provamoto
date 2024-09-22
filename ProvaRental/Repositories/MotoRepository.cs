using Microsoft.EntityFrameworkCore;

public class MotoRepository : IMotoRepository
{
    private readonly ProvaContext _context;

    public MotoRepository(ProvaContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Moto>> GetMotosAsync() => await _context.Motos.ToListAsync();

    public async Task<Moto> GetMotoByIdAsync(int id) => await _context.Motos.FindAsync(id);

    public async Task AddMotoAsync(Moto moto)
    {
        await _context.Motos.AddAsync(moto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateMotoAsync(Moto moto)
    {
        _context.Motos.Update(moto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMotoAsync(int id)
    {
        var moto = await _context.Motos.FindAsync(id);
        if (moto != null)
        {
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();
        }
    }
}

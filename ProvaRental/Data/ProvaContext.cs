using Microsoft.EntityFrameworkCore;
using ProvaRental.Data.Models;

public class ProvaContext : DbContext
{
    public ProvaContext(DbContextOptions<ProvaContext> options) : base(options) { }

    public DbSet<Moto> Motos { get; set; }
    public DbSet<Entregador> Entregadores { get; set; }
    public DbSet<Locacao> Locacoes { get; set; }
    public DbSet<UserLogin> UserLogin { get; set; }
    
}
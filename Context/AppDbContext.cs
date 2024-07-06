using FitNotionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FitNotionApi.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Usuarios> Usuarios { get; set; }

        public DbSet<Alimentos> Alimentos { get; set; }

        public DbSet<ConsumoDiario> ConsumoDiario { get; set; }

        public DbSet<ConsumoDetalle> ConsumoDetalle { get; set; }

        public DbSet<Nutricionistas> Nutricionistas { get; set; }

        public DbSet<Clientes> Clientes { get; set; }

        public DbSet<Dietas> Dietas { get; set; }

        public DbSet<ComidasDieta> ComidasDieta { get; set; }

        public DbSet<NotasDieta> NotasDieta { get; set;}

    }
}

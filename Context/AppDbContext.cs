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
    }
}

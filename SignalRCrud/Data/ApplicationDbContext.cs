using Microsoft.EntityFrameworkCore;
using SignalRCrud.Models;

namespace SignalRCrud.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){

        }
        public DbSet<Products> Product { get; set; }

    }
}

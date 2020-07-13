using Microsoft.EntityFrameworkCore;
using STOCK.API.Core.Model;

namespace STOCK.API.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Broker> Broker { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockPrice> StockPrice { get; set; }

    }
}
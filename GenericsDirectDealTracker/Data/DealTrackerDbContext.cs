using Microsoft.EntityFrameworkCore;
using GenericsDirectDealTracker.Models;

namespace GenericsDirectDealTracker.Data
{
    public class DealTrackerDbContext : DbContext
    {
        public DealTrackerDbContext(DbContextOptions<DealTrackerDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<DealScenario> DealScenarios { get; set; }
        public DbSet<DealScenarioDetail> DealScenarioDetails { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
    }
}

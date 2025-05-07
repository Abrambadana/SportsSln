using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SportsStore.Models
{
    public class StoreDBContextFactory : IDesignTimeDbContextFactory<StoreDBContext>
    {
        public StoreDBContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<StoreDBContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("SportsStoreConnection"));

            return new StoreDBContext(optionsBuilder.Options);
        }
    }
}

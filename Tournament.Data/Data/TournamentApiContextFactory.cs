using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace Tournament.Data.Data
{
    public class TournamentApiContextFactory : IDesignTimeDbContextFactory<TournamentApiContext>
    {
        public TournamentApiContext CreateDbContext(string[] args)
        {
            // Find the root directory that contains appsettings.json
            var basePath = Directory.GetCurrentDirectory();
            var rootPath = basePath;

            while (!File.Exists(Path.Combine(rootPath, "appsettings.json")))
            {
                rootPath = Directory.GetParent(rootPath)?.FullName
                           ?? throw new FileNotFoundException("Could not find appsettings.json while walking up the directory tree.");
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TournamentApiContext>();
            var connectionString = config.GetConnectionString("TournamentApiContext");

            optionsBuilder.UseSqlServer(connectionString);

            return new TournamentApiContext(optionsBuilder.Options);
        }
    }
}

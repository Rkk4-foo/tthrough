using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TThrough.data
{
    public class TalkthroughContextFactory : IDesignTimeDbContextFactory<TalkthroughContext>
    {
        public TalkthroughContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TalkthroughContext>();
            var connectionString = configuration.GetConnectionString("TalkthroughDatabase");

            optionsBuilder.UseSqlServer(connectionString);

            return new TalkthroughContext(optionsBuilder.Options);
        }


        //Crea el contexto de la base de datos para enviarlo a los viewmodels
        public static TalkthroughContext SendContextFactory() 
        {
            return new TalkthroughContextFactory().CreateDbContext(Array.Empty<string>());
        }
    }
}
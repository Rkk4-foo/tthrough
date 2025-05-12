using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using TThrough.data;

namespace TThrough
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();

            // Leer configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Registrar TalkthroughContext con la cadena de conexión
            serviceCollection.AddDbContext<TalkthroughContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TalkthroughDatabase")));

            // Agrega otros servicios, viewmodels, etc. si quieres
            // serviceCollection.AddTransient<MainViewModel>();

            Services = serviceCollection.BuildServiceProvider();

            base.OnStartup(e);
        }
    }

}

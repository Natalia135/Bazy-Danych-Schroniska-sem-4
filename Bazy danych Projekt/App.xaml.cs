using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Bazy_danych_Projekt.Data;
using Bazy_danych_Projekt.ViewModels;
using Bazy_danych_Projekt.Views;

namespace Bazy_danych_Projekt
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();

            // Konfiguracja bazy danych (LocalDB)
            services.AddDbContext<SchroniskoDbContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SchroniskoAppDb;Trusted_Connection=True;");
            });

            // Rejestracja MVVM
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            // --- NOWE OKNA ---
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginWindow>();

            services.AddTransient<ZwierzeSzczegolyViewModel>();
            services.AddTransient<ZwierzeSzczegolyWindow>();

            services.AddTransient<DodajZwierzeViewModel>();
            services.AddTransient<DodajZwierzeWindow>();

            services.AddTransient<WnioskiAdopcyjneViewModel>();
            services.AddTransient<WnioskiAdopcyjneWindow>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UniNote.DBMigration;
using UniOrm.Application;
using UniOrm.Startup.WPF;

namespace CodeGenergator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ServiceProvider _serviceProvider;
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureServices();
            //DbMigrationUnit.EnsureDaContext( appConfig.UsingDBConfig,typeof(MigrationVersion1).Assembly);
            services.AddSingleton<MainWindow>();
        }
        //private void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    var main = _serviceProvider.GetRequiredService<MainWindow>();
        //    main.Show();
            
        //} 
       
    }
}

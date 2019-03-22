using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Core;
using MyApp.Core.Contracts;
using MyApp.Data;
using System;

namespace MyApp
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            IServiceProvider services = ConfigureServices();
            IEngine engine = new Engine(services);
            engine.Run();

            //Database
            //CommandPattern
            //implement Dependancy Injection
            //DTOs
            //Service Layer
        }

        private static IServiceProvider ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddDbContext<MyAppContext>(db => db.UseSqlServer("Server=.;Database=MySpecialApp;Integrated Security=True"));

            serviceCollection.AddTransient<ICommandInterpreter, CommandInterpreter>();
            serviceCollection.AddTransient<Mapper>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}

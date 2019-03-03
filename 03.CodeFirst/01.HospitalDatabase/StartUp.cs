using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using P01_HospitalDatabase.Data;
using System;

namespace P01_HospitalDatabase 
{
    public class StartUp
    {
        public static void Main()
        {
            //using (var dbContext = new HospitalContext())
            //{
            //    DatabaseInitializer.ResetDatabase(dbContext);
            //}

            LoggerFactory SqlCommandLoggerFactory = new LoggerFactory(new[]
            {
                new ConsoleLoggerProvider((category, level)
                    => category == DbLoggerCategory.Database.Command.Name 
                    && level == LogLevel.Information, true)
            });

            DbContextOptionsBuilder<HospitalContext> optionBuilder = new DbContextOptionsBuilder<HospitalContext>();

            optionBuilder
                .UseSqlServer(Configuration.ConnectionString, s => s.MigrationsAssembly("01.HospitalDatabase"))
                .UseLoggerFactory(SqlCommandLoggerFactory)
                .EnableSensitiveDataLogging();

        }
    }
}

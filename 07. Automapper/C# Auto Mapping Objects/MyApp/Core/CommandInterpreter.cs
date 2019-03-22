using Microsoft.Extensions.DependencyInjection;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyApp.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";
        private readonly IServiceProvider serviceProvider;

        public CommandInterpreter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public string Read(string[] inputArgs)
        {
            string commandName = inputArgs[0] + Suffix; //Here we'll receive command.. example: AddEmployee

            string[] commandParams = inputArgs.Skip(1).ToArray();

            var type = Assembly.GetCallingAssembly()
                               .GetTypes()
                               .FirstOrDefault(x => x.Name == commandName);

            if (type == null)
            {
                throw new ArgumentNullException("Invalid Command");
            }

            var constructor = type.GetConstructors()
                                  .FirstOrDefault();

            var constructorParams = constructor.GetParameters()
                                               .Select(x => x.ParameterType)
                                               .ToArray();

            var services = constructorParams.Select(this.serviceProvider.GetService)
                                            .ToArray();

            
            var command = (ICommand)constructor.Invoke(services);

            string result = command.Execute(commandParams);

            return result;
        }
    }
}

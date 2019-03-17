using BillsPaymentSystem.App.Core.Command.Contracts;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BillsPaymentSystem.App.Core
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private const string Suffix = "Command";

        public string Read(string[] args, BillsPaymentSystemContext context)
        {

        
            string command = args[0];
            string[] commandArgs = args.Skip(1).ToArray();

            var type = Assembly.GetCallingAssembly()
                               .GetTypes()
                               .FirstOrDefault(x => x.Name == command + Suffix);

            if (type == null)
            {
                throw new ArgumentNullException("Command not found!");
            }

            var typeInstance = Activator.CreateInstance(type, context);

            var result = ((ICommand)typeInstance).Execute(commandArgs);

            return result;

        }
    }
}

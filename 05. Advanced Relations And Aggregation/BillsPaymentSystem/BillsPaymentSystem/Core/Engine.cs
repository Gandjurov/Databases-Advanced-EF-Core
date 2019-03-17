using BillsPaymentSystem.App.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.App.Core
{
    public class Engine : IEngine
    {
        private readonly ICommandInterpreter commandInterpreter;

        public Engine(ICommandInterpreter commandInterpreter)
        {
            this.commandInterpreter = commandInterpreter;
        }

        public void Run()
        {
            while (true)
            {
                string[] inputParams = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                string result = this.commandInterpreter.Read(inputParams);

                //TODO: Catch exceptions
            }
        }
    }
}

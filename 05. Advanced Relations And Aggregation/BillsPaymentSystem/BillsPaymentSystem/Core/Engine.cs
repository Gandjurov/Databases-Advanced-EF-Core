using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
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
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                string[] inputParams = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
                {
                    string result = this.commandInterpreter.Read(inputParams, context);
                    Console.WriteLine(result);
                }
                
            }


        }
    }
}

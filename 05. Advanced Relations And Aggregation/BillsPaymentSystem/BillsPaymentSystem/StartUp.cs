﻿using BillsPaymentSystem.App.Core;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Linq;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        public static void Main()
        {
            //using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            //{
            //    DbInitilizer.Seed(context);
            //}
            
            ICommandInterpreter commandInterpreter = new CommandInterpreter();

            IEngine engine = new Engine(commandInterpreter);
            engine.Run();

        }

        
    }
}

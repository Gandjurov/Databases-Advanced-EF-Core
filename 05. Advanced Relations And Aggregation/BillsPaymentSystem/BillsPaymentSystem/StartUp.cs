using BillsPaymentSystem.App.Core;
using BillsPaymentSystem.App.Core.Contracts;
using BillsPaymentSystem.Data;
using System;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        public static void Main()
        {
            IEngine engine = new Engine();
            engine.Run();
        }
    }
}

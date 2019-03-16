using BillsPaymentSystem.Data;
using System;

namespace BillsPaymentSystem.App
{
    public class StartUp
    {
        public static void Main()
        {
            using (BillsPaymentSystemContext context = new BillsPaymentSystemContext())
            {
                DbInitilizer.Seed(context);
            }
        }
    }
}

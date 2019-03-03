using P01_HospitalDatabase.Data;
using System;

namespace P01_HospitalDatabase 
{
    public class StartUp
    {
        public static void Main()
        {
            using (var dbContext = new HospitalContext())
            {
                DatabaseInitializer.ResetDatabase(dbContext);
            }
        }
    }
}

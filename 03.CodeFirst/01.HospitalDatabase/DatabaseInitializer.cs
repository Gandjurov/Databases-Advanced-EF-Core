namespace P01_HospitalDatabase
{
    using P01_HospitalDatabase.Data;
    using P01_HospitalDatabase.Generators;
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Text;
    
    public class DatabaseInitializer
    {
        private static Random rnd = new Random();

        public static void ResetDatabase(HospitalContext context)
        {
            context.Database.EnsureDeleted();

            context.Database.EnsureCreated();

            InitialSeed(context);
        }

        private static void InitialSeed(HospitalContext context)
        {
            SeedMedicaments(context);

            SeedPatient(context, 200);

            SeedPrescriptions(context);

        }

        private static void SeedPrescriptions(HospitalContext context)
        {
            PrescriptionGenerator.InitialPrescriptionSeed(context);
        }

        private static void SeedPatient(HospitalContext context, int count)
        {
            for (int i = 0; i < count; i++)
            {
                context.Patients.Add(PatientGenerator.NewPatient(context));
            }
        }

        private static void SeedMedicaments(HospitalContext context)
        {
            MedicamentGenerator.InitialMedicamentSeed(context);
        }


    }
}

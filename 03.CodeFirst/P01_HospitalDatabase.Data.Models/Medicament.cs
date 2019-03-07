using System;
using System.Collections.Generic;
using System.Text;

namespace P01_HospitalDatabase.Data.Models
{
    public class Medicament
    {
        public Medicament()
        {
            this.Presciptions = new HashSet<PatientMedicament>();
        }

        //[Key]
        public int MedicamentId { get; set; }

        //[StringLength(50)]
        public string Name { get; set; }

        public ICollection<PatientMedicament> Presciptions { get; set; }
    }
}

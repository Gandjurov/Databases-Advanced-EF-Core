using System.ComponentModel.DataAnnotations;

namespace PetClinic.Data.Models
{
    public class ProcedureAnimalAid
    {
        [Required]
        public int ProcedureId { get; set; }
        public Procedure Procedure { get; set; }

        [Required]
        public int AnimalAidId { get; set; }
        public AnimalAid AnimalAid { get; set; }
    }
}
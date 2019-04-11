using System;
using System.ComponentModel.DataAnnotations;

namespace PetClinic.Data.Models
{
    public class Passport
    {
        [Key]
        [RegularExpression(@"^[A-Za-z]{7}[0-9]{3}$")]
        public string SerialNumber { get; set; }

        [Required]
        public Animal Animal { get; set; }

        [Required]
        [RegularExpression(@"^(\+359|0)[0-9]{9}$")]
        public int OwnerPhoneNumber { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string OwnerName { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }
    }
}
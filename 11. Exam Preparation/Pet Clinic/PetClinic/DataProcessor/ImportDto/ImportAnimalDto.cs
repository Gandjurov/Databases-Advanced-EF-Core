using PetClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetClinic.DataProcessor.ImportDto
{
    public class ImportAnimalDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Type { get; set; }

        [Range(1, 666)]
        public int Age { get; set; }

        [Required]
        public PassportDto Passport { get; set; }
    }
}

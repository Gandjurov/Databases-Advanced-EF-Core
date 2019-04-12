using PetClinic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.ImportDto
{
    [XmlType("Procedure")]
    public class ImportProceduresDto
    {
        [Required]
        [XmlElement("Vet")]
        public string VetName { get; set; }

        [Required]
        [XmlElement("Animal")]
        public string AnimalPassportSN { get; set; }

        [Required]
        [XmlElement("DateTime")]
        public string DateTime { get; set; }

        [XmlArray("AnimalAids")]
        public AnimalAidDto[] AnimalAids { get; set; }
    }
}

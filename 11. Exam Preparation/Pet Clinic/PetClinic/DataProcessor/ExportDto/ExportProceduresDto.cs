using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PetClinic.DataProcessor.ExportDto
{
    [XmlType("Procedure")]
    public class ExportProceduresDto
    {
        [XmlElement("Passport")]
        public string AnimalSerialNumber { get; set; }

        [XmlElement("OwnerNumber")]
        public string OwnerNumber { get; set; }

        [XmlElement("DateTime")]
        public string ProcedureDate { get; set; }

        [XmlArray("AnimalAids")]
        public ExportAnimalAidDto[] AnimalAids { get; set; }

        [XmlElement("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}

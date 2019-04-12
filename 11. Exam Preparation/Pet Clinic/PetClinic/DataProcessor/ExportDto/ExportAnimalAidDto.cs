using System.Xml.Serialization;

namespace PetClinic.DataProcessor.ExportDto
{
    [XmlType("AnimalAid")]
    public class ExportAnimalAidDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
using System.Xml.Serialization;

namespace CarDealer.Dtos.Export
{
    [XmlType("part")]
    public class ExportPartsDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlAttribute("price")]
        public decimal Price { get; set; }
    }
}
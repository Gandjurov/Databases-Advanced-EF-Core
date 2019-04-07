using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("Purchase")]
    public class ExportPurchaseDto
    {
        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("Cvc")]
        public string CVC { get; set; }

        [XmlElement("Date")]
        public string Date { get; set; }


        public ExportGameDto Game { get; set; }
    }
}
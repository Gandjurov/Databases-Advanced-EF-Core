using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Mail : BaseModel<int>
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s]+ str.$")]
        public string Address { get; set; }

        public int PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }
    }
}
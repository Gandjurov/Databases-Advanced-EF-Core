using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SoftJail.Data.Models
{
    public class Department : BaseModel<int>
    {
        public Department()
        {
            this.Cells = new List<Cell>();
        }

        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }

        public ICollection<Cell> Cells { get; set; }
    }
}
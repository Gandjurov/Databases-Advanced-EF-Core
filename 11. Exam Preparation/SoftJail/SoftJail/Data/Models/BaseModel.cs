using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.Data.Models
{
    public abstract class BaseModel<T>
    {
        [Key]
        public T Id { get; set; }
    }
}

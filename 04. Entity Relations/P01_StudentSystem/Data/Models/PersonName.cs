using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class PersonName
    {
        public PersonName(string firstName, string lastName)
        {
            //if (firstName.Length + lastName.Length > 99)
            //{
            //    throw new ArgumentException("Name must be not longer than 100 symbols");
            //}

            FirstName = firstName;
            LastName = lastName;
        }

        [Column("FirstName")]
        [Required]
        public string FirstName { get; set; }

        [Column("LastName")]
        [Required]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }
    }
}

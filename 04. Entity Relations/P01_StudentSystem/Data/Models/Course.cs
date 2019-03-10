using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course()
        {
            Resources = new List<Resource>();
            Students = new List<StudentCourse>();
        }

        [Key]
        public int CourseId { get; set; }

        [Column(TypeName = "NVARCHAR(80)")]
        [Required]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        //[Column(TypeName = "MONEY")]
        public decimal Price { get; set; }

        public List<Resource> Resources { get; set; }

        public List<StudentCourse> Students { get; set; }
    }
}

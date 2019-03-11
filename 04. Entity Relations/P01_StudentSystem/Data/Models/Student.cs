using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student(
            string name,
            string phoneNumber = null,
            DateTime? birthday = null 
            )
        {
            this.Name = Name;
            this.PhoneNumber = phoneNumber;
            this.RegisteredOn = DateTime.Now;

            this.CourseEnrollments = new List<StudentCourse>();
            this.HomeworkSubmissions = new List<Homework>();
        }

        public int StudentId { get; set; }

        public PersonName Name { get; private set; }

        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; private set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }

        public ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}

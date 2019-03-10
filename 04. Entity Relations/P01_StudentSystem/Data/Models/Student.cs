﻿using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Student
    {
        public Student()
        {
            StudentCourses = new List<StudentCourse>();
        }

        public int StudentId { get; set; }

        public PersonName Name { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get; set; }

        public DateTime? Birthday { get; set; }

        public List<StudentCourse> StudentCourses { get; set; }
    }
}

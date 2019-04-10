namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var allDepartments = JsonConvert.DeserializeObject<Department[]>(jsonString);
            var sb = new StringBuilder();
            var validDepartments = new List<Department>();

            foreach (var department in allDepartments)
            {
                var isValid = IsValid(department) &&
                    department.Cells.All(IsValid);

                if (isValid)
                {
                    validDepartments.Add(department);
                    sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
                }
                else
                {
                    sb.AppendLine($"Invalid Data");
                }
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            throw new NotImplementedException();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}
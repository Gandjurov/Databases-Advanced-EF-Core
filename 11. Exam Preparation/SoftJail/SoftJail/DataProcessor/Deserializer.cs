namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
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
            var prisonersDto = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);
            var sb = new StringBuilder();
            var validPrisoners = new List<Prisoner>();

            foreach (var dto in prisonersDto)
            {
                var isValid = IsValid(dto) &&
                      dto.Mails.All(IsValid);

                if (isValid)
                {
                    var releaseDate = dto.ReleaseDate == null
                            ? new DateTime?()
                            : DateTime.ParseExact(
                                dto.ReleaseDate,
                                "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);

                    var prisoner = new Prisoner
                    {
                        FullName = dto.FullName,
                        Nickname = dto.Nickname,
                        Age = dto.Age,
                        IncarcerationDate = DateTime.ParseExact(
                            dto.IncarcerationDate,
                            "dd/MM/yyyy",
                            CultureInfo.InvariantCulture),
                        ReleaseDate = releaseDate,
                        Bail = dto.Bail,
                        CellId = dto.CellId,
                        Mails = dto.Mails
                                   .Select(m =>
                                        new Mail
                                        {
                                            Description = m.Description,
                                            Sender = m.Sender,
                                            Address = m.Address
                                        })
                                   .ToArray(),
                    };

                    validPrisoners.Add(prisoner);
                    sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                }
                else
                {
                    sb.AppendLine($"Invalid Data");
                }
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
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
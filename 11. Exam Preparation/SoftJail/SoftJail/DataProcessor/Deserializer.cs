namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

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
            var serializer = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));
            var allOfficers = (ImportOfficerDto[])serializer.Deserialize(new StringReader(xmlString));

            var validOfficers = new List<Officer>();
            var sb = new StringBuilder();

            foreach (var dto in allOfficers)
            {
                var isWeaponValid = Enum.TryParse<Weapon>(dto.Weapon, out Weapon weapon);
                var isPositionValid = Enum.TryParse<Position>(dto.Position, out Position position);

                var isValid = IsValid(dto) && isWeaponValid && isPositionValid;

                if (isValid)
                {

                    var officer = new Officer
                    {
                        FullName = dto.Name,
                        Salary = dto.Money,
                        Position = position,
                        Weapon = weapon,
                        DepartmentId = dto.DepartmentId,
                        OfficerPrisoners = dto.Prisoners
                                    .Select(p => new OfficerPrisoner
                                    {
                                        PrisonerId = p.Id
                                    })
                                    .ToArray()
                    };

                    validOfficers.Add(officer);
                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
                else
                {
                    sb.AppendLine($"Invalid Data");
                }
            }


            context.Officers.AddRange(validOfficers);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, validationResults, true);
        }
    }
}
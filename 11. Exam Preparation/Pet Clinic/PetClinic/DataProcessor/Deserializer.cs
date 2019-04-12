namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.DataProcessor;
    using PetClinic.DataProcessor.ImportDto;
    using PetClinic.Models;

    public class Deserializer
    {

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            AnimalAid[] animalAids = JsonConvert.DeserializeObject<AnimalAid[]>(jsonString);

            var validEntries = new List<AnimalAid>();

            var sb = new StringBuilder();

            foreach (var dto in animalAids)
            {
                bool isValid = IsValid(dto);

                bool alreadyExists = validEntries.Any(a => a.Name == dto.Name);

                if (!isValid || alreadyExists)
                {
                    sb.AppendLine($"Error: Invalid data.");
                    continue;
                }

                validEntries.Add(dto);
                sb.AppendLine($"Record {dto.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var animals = JsonConvert.DeserializeObject<ImportAnimalDto[]>(jsonString);

            var validEntries = new List<Animal>();

            var sb = new StringBuilder();

            foreach (var dto in animals)
            {
                bool isValid = IsValid(dto) && IsValid(dto.Passport);
                bool alreadyExists = validEntries.Any(sn => sn.Passport.SerialNumber == dto.Passport.SerialNumber);

                if (!isValid || alreadyExists)
                {
                    sb.AppendLine($"Error: Invalid data.");
                    continue;
                }

                var animal = new Animal
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    Age = dto.Age,
                    Passport = new Passport
                    {
                        SerialNumber = dto.Passport.SerialNumber,
                        OwnerName = dto.Passport.OwnerName,
                        OwnerPhoneNumber = dto.Passport.OwnerPhoneNumber,
                        RegistrationDate = DateTime.ParseExact(
                                           dto.Passport.RegistrationDate,
                                           "dd-MM-yyyy",
                                           CultureInfo.InvariantCulture)
                    }
                };
                
                validEntries.Add(animal);
                sb.AppendLine($"Record {animal.Name} Passport №: {animal.Passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(validEntries);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportVetsDto[]), new XmlRootAttribute("Vets"));
            var allVets = (ImportVetsDto[])serializer.Deserialize(new StringReader(xmlString));

            var validVets = new List<Vet>();
            var sb = new StringBuilder();

            foreach (var dto in allVets)
            {
                bool alreadyExists = validVets.Any(pn => pn.PhoneNumber == dto.PhoneNumber);
                var isValid = IsValid(dto);

                if (!isValid || alreadyExists)
                {
                    sb.AppendLine($"Error: Invalid data.");
                    continue;
                }

                var vet = new Vet
                {
                    Name = dto.Name,
                    Profession = dto.Profession,
                    Age = dto.Age,
                    PhoneNumber = dto.PhoneNumber
                };

                validVets.Add(vet);
                sb.AppendLine($"Record {vet.Name} successfully imported.");
            }

            context.Vets.AddRange(validVets);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportProceduresDto[]), new XmlRootAttribute("Procedures"));
            var allProcedures = (ImportProceduresDto[])serializer.Deserialize(new StringReader(xmlString));

            var validProcedures = new List<Procedure>();

            var sb = new StringBuilder();

            foreach (var dto in allProcedures)
            {
                DateTime procedureDate;
                var isProcDateValid = DateTime.TryParseExact(dto.DateTime,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out procedureDate);

                if (!IsValid(dto) || dto.AnimalAids.Length != dto.AnimalAids.Select(a => a.Name).Distinct().Count())
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var vet = context.Vets.FirstOrDefault(v => v.Name.Equals(dto.VetName, StringComparison.OrdinalIgnoreCase));

                if (vet == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var animal = context.Animals.FirstOrDefault(a => a.PassportSerialNumber.Equals(dto.AnimalPassportSN, StringComparison.OrdinalIgnoreCase));

                if (animal == null)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var aids = context.AnimalAids
                                  .Where(a => dto.AnimalAids.Any(dtoAid => dtoAid.Name.Equals(a.Name, StringComparison.OrdinalIgnoreCase)))
                                  .ToArray();

                if (aids.Length != dto.AnimalAids.Length)
                {
                    sb.AppendLine("Error: Invalid data.");
                    continue;
                }

                var mappings = new List<ProcedureAnimalAid>();
                foreach (var aid in aids)
                {
                    mappings.Add(new ProcedureAnimalAid
                    {
                        AnimalAidId = aid.Id
                    });
                }

                var procedure = new Procedure
                {
                    VetId = vet.Id,
                    AnimalId = animal.Id,
                    DateTime = procedureDate,
                    ProcedureAnimalAids = mappings
                };
            }

            context.Procedures.AddRange(validProcedures);
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

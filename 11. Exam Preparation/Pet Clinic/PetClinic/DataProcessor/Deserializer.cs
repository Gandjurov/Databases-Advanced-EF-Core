namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
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
            throw new NotImplementedException();
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
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

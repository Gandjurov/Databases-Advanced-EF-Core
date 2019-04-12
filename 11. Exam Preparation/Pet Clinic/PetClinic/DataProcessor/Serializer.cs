namespace PetClinic.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using PetClinic.Data;
    using PetClinic.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animalsByOwner = context.Animals
                                .Where(p => p.Passport.OwnerPhoneNumber == phoneNumber)
                                .Select(a => new
                                {
                                    OwnerName = a.Passport.OwnerName,
                                    AnimalName = a.Passport.Animal.Name,
                                    Age = a.Passport.Animal.Age,
                                    SerialNumber = a.Passport.SerialNumber,
                                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InstalledUICulture)
                                })
                                .OrderBy(a => a.Age)
                                .ThenBy(a => a.SerialNumber)
                                .ToArray();

            var json = JsonConvert.SerializeObject(animalsByOwner, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var sb = new StringBuilder();

            var allProcedures = context.Procedures
                                    .Select(p => new
                                    {
                                        Passport = p.Animal.Passport.SerialNumber,
                                        OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                                        DateTime = p.DateTime,
                                        AnimalAids = p.ProcedureAnimalAids
                                            .Select(paa => paa.AnimalAid)
                                            .Select(aa => new ExportAnimalAidDto
                                            {
                                                Name = aa.Name,
                                                Price = aa.Price
                                            })
                                            .ToArray(),
                                        TotalPrice = p.Cost
                                    })
                                    .OrderBy(p => p.DateTime)
                                    .ThenBy(p => p.Passport)
                                    .Select(p => new ExportProceduresDto
                                    {
                                        AnimalSerialNumber = p.Passport,
                                        OwnerNumber = p.OwnerNumber,
                                        ProcedureDate = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                                        AnimalAids = p.AnimalAids,
                                        TotalPrice = p.TotalPrice
                                    })
                                .ToArray();
            

            var serializer = new XmlSerializer(typeof(ExportProceduresDto[]), new XmlRootAttribute("Procedures"));

            serializer.Serialize
                (
                    new StringWriter(sb),
                    allProcedures,
                    new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty })
                );

            var result = sb.ToString().TrimEnd();

            return result;
        }
    }
}

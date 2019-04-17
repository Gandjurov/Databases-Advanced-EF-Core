namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var movies = context.Movies
                                .Where(m => m.Rating >= rating && m.Projections.Any(t => t.Tickets.Count >= 1))
                                .OrderByDescending(r => r.Rating)
                                .ThenByDescending(p => p.Projections.Sum(t => t.Tickets.Sum(pc => pc.Price)))
                                .Select(x => new
                                {
                                    MovieName = x.Title,
                                    Rating = x.Rating.ToString("F2"),
                                    TotalIncomes = x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                                    Customers = x.Projections.SelectMany(t => t.Tickets).Select(c => new
                                    {
                                        FirstName = c.Customer.FirstName,
                                        LastName = c.Customer.LastName,
                                        Balance = c.Customer.Balance.ToString("F2")
                                    })
                                    .OrderByDescending(b => b.Balance)
                                    .ThenBy(f => f.FirstName)
                                    .ThenBy(l => l.LastName)
                                    .ToArray()
                                })
                                .Take(10)
                                .ToArray();

            var jsonResult = JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);

            return jsonResult;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                                   .Where(c => c.Age >= age)
                                   .OrderByDescending(x => x.Tickets.Sum(p => p.Price))
                                   .Take(10)
                                   .Select(x => new ExportCustomersDto
                                   {
                                       FirstName = x.FirstName,
                                       LastName = x.LastName,
                                       SpentMoney = x.Tickets.Sum(p => p.Price).ToString("F2"),
                                       SpentTime = TimeSpan.FromSeconds(
                                                        x.Tickets.Sum(s => s.Projection.Movie.Duration.TotalSeconds))
                                                    .ToString(@"hh\:mm\:ss")
                                   })
                                   .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportCustomersDto[]), new XmlRootAttribute("Customers"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();

        }
    }
}
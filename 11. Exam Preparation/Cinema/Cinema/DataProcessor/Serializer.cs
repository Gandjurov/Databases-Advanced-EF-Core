namespace Cinema.DataProcessor
{
    using System;
    using System.Linq;
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
            throw new NotImplementedException();
        }
    }
}
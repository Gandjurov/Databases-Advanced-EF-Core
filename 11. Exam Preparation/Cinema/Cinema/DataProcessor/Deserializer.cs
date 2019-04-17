namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie 
            = "Successfully imported {0} with genre {1} and rating {2:F2}!";
        private const string SuccessfulImportHallSeat 
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection 
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket 
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var moviesDto = JsonConvert.DeserializeObject<ImportMoviesDto[]>(jsonString);
            var sb = new StringBuilder();
            var movies = new List<Movie>();

            foreach (var dto in moviesDto)
            {
                var movieTitle = context.Movies.Any(x => x.Title == dto.Title);

                var isValidEnum = Enum.TryParse(typeof(Genre), dto.Genre, out object genreType);
                
                if (!IsValid(dto) || !isValidEnum || movieTitle == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = new Movie
                {
                    Title = dto.Title,
                    Genre = (Genre)genreType,
                    Duration = dto.Duration,
                    Rating = dto.Rating,
                    Director = dto.Director
                };

                movies.Add(movie);
                sb.AppendLine(string.Format(SuccessfulImportMovie, movie.Title, movie.Genre, movie.Rating));
            }

            context.Movies.AddRange(movies);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var hallSeatsDto = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);
            var sb = new StringBuilder();
            var hallSeats = new List<Hall>();

            foreach (var dto in hallSeatsDto)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hallSeat = new Hall
                {
                    Name = dto.Name,
                    Is4Dx = dto.Is4Dx,
                    Is3D = dto.Is3D
                };

                for (int i = 0; i < dto.Seats; i++)
                {
                    hallSeat.Seats.Add(new Seat());
                }

                hallSeats.Add(hallSeat);

                string status = "";

                if (hallSeat.Is4Dx)
                {
                    status = hallSeat.Is3D ? "4Dx/3D" : "4Dx";
                }
                else if (hallSeat.Is3D)
                {
                    status = "3D";
                }
                else
                {
                    status = "Normal";
                }

                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hallSeat.Name, status, hallSeat.Seats.Count));
            }

            context.Halls.AddRange(hallSeats);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var projections = new List<Projection>();

            var xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDto[]), new XmlRootAttribute("Projections"));

            var projectionsDto = (ImportProjectionsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            foreach (var dto in projectionsDto)
            {
                var movieId = context.Movies.FirstOrDefault(m => m.Id == dto.MovieId);
                var hallId = context.Halls.FirstOrDefault(h => h.Id == dto.HallId);


                if (!IsValid(dto) || movieId == null || hallId == null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                

                var projection = new Projection
                {
                    MovieId = dto.MovieId,
                    HallId = dto.HallId,
                    DateTime = DateTime.ParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                };

                projections.Add(projection);

                var movieTitle = context.Movies.FirstOrDefault(m => m.Id == dto.MovieId).Title;
                var projectionTime = projection.DateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);

                sb.AppendLine(string.Format(SuccessfulImportProjection, movieTitle, projectionTime));
            }

            context.Projections.AddRange(projections);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ImportCustomerTicketsDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomerTicketsDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var customers = new List<Customer>();
            var sb = new StringBuilder();

            foreach (var dto in customersDto)
            {
                var projections = context.Projections.Select(x => x.Id).ToArray();
                var projectionExists = projections.Any(x => dto.Tickets.Any(s => s.ProjectionId != x));

                if (!IsValid(dto) && dto.Tickets.All(IsValid) && projectionExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var customer = new Customer
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    Balance = dto.Balance
                };

                foreach (var ticket in dto.Tickets)
                {
                    customer.Tickets.Add(new Ticket
                    {
                        ProjectionId = ticket.ProjectionId,
                        Price = ticket.Price
                    });
                }

                customers.Add(customer);

                sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return sb.ToString();

        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResult, true);

            return isValid;
        }
    }
}
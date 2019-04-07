using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VaporStore.Data.Models.Enums;
using VaporStore.DataProcessor.ExportDtos;
using Formatting = Newtonsoft.Json.Formatting;

namespace VaporStore.DataProcessor
{
    using System;
    using Data;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context
                .Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games
                        .Where(p => p.Purchases.Any())
                        .Select(g => new
                        {
                            Id = g.Id,
                            Title = g.Name,
                            Developer = g.Developer.Name,
                            Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name).ToArray()),
                            Players = g.Purchases.Count
                        })
                        .OrderByDescending(p => p.Players)
                        .ThenBy(g => g.Id)
                        .ToArray(),
                    TotalPlayers = x.Games.Sum(p => p.Purchases.Count)
                })
                .OrderByDescending(t => t.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToArray();

            var jsonResult = JsonConvert.SerializeObject(genres, Formatting.Indented);

            return jsonResult;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var purchaseType = Enum.Parse<PurchaseType>(storeType);

            var users = context
                .Users
                .Select(x => new ExportUserDto
                {
                    Username = x.Username,
                    Purchases = x.Cards
                        .SelectMany(p => p.Purchases)
                        .Where(t => t.Type == purchaseType)
                        .Select(p => new ExportPurchaseDto
                    {
                        Card = p.Card.Number,
                        Cvc = p.Card.Cvc,
                        Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new ExportGameDto
                        {
                            Genre = p.Game.Genre.Name,
                            Title = p.Game.Name,
                            Price = p.Game.Price
                        }
                    })
                        .OrderBy(d => d.Date)
                        .ToArray(),
                    TotalSpent = x.Cards.SelectMany(p => p.Purchases)
                        .Where(t => t.Type == purchaseType)
                        .Sum(p => p.Game.Price)
                })
                .Where(p => p.Purchases.Any())
                .OrderByDescending(t => t.TotalSpent)
                .ThenBy(u => u.Username)
                .ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces(new []
            {
                XmlQualifiedName.Empty,  
            });

            var sb = new StringBuilder();
            xmlSerializer.Serialize(new StringWriter(sb), users, namespaces);

            var result = sb.ToString().TrimEnd();

            return result;
        }
    }
}
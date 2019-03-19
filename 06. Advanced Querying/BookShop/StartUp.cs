namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //DbInitializer.ResetDatabase(db);

                //var result = GetBooksByAgeRestriction(db, "teEN");

                //var result = GetGoldenBooks(db);

                //var result = GetBooksByPrice(db);
                
                var result = GetBooksNotReleasedIn(db, 1998);

                Console.WriteLine(result);

            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                               .Where(a => a.AgeRestriction == ageRestriction)
                               .Select(t => t.Title)
                               .OrderBy(x => x)
                               .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                   .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                   .OrderBy(b => b.BookId)
                   .Select(b => b.Title)
                   .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                               .Where(b => b.Price > 40)
                               .OrderByDescending(x => x.Price)
                               .Select(b => new { b.Title, b.Price })
                               .ToList();


            var result = string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - ${b.Price:F2}"));

            return result;
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                   .Where(b => b.ReleaseDate.Value.Year != year)
                   .OrderBy(b => b.BookId)
                   .Select(b => b.Title)
                   .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }
    }
}

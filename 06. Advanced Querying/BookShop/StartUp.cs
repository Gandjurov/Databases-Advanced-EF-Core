namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
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

                //var result = GetBooksNotReleasedIn(db, 1998);

                //string input = "horror mystery drama";
                //var result = GetBooksByCategory(db, input);

                //var result = GetBooksReleasedBefore(db, "12-04-1992");

                //var stringInput = "dy";
                //var result = GetAuthorNamesEndingIn(db, stringInput);

                //var stringInput = "sK";
                //var result = GetBookTitlesContaining(db, stringInput);

                //var stringInput = "po";
                //var result = GetBooksByAuthor(db, stringInput);

                var result = CountBooks(db, 12);

                Console.WriteLine(result);

            }
        }

        //01.	Age Restriction
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

        //02.	Golden Books
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

        //03.	Books by Price
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

        //04.	Not Released In
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

        //05.	Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {

            string[] categories = input.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(c => c.ToLower()).ToArray();
            var books = context.Books
                               .Where(b => b.BookCategories
                                    .Any(bc => categories.Contains(bc.Category.Name.ToLower())))
                               .OrderBy(b => b.Title)
                               .Select(b => b.Title)
                               .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        //06.	Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime releaseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                   .Where(b => b.ReleaseDate < releaseDate)
                   .OrderByDescending(b => b.ReleaseDate)
                   .Select(b => new { b.Title, b.EditionType, b.Price })
                   .ToList();

            var result = string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}"));

            return result;
        }

        //07.	Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                                .Where(a => a.FirstName.EndsWith(input))
                                .Select(a => $"{a.FirstName} {a.LastName}")
                                .OrderBy(a => a)
                                .ToList();

            var result = string.Join(Environment.NewLine, authors);

            return result;
        }

        //08.	Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var inputString = input.ToLower();

            var books = context.Books
                               .Where(b => b.Title.ToLower().Contains(inputString))
                               .Select(b => b.Title)
                               .OrderBy(t => t)
                               .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        //09.	Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var inputString = input.ToLower();

            var books = context.Books
                   .Where(b => b.Author.LastName.ToLower().StartsWith(inputString))
                   .OrderBy(b => b.BookId)
                   .Select(b => new { b.Title, b.Author })
                   .ToList();

            var result = string.Join(Environment.NewLine, books.Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})"));

            return result;
        }

        //10.	Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                   .Where(b => b.Title.Count() > lengthCheck)
                   .Count();
            

            return books;
        }
    }
}

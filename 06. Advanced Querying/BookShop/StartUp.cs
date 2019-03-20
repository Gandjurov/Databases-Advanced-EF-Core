namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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

                //var result = CountBooks(db, 12);


                //var result = CountCopiesByAuthor(db);

                //var result = GetTotalProfitByCategory(db);

                //var result = GetMostRecentBooks(db);

                var result = RemoveBooks(db);
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

        //11.	Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                    .Select(a => new
                    {
                        Name = $"{a.FirstName} {a.LastName}",
                        Copies = a.Books.Select(b => b.Copies).Sum()
                    })
                    .OrderByDescending(a => a.Copies)
                    .ToList();

            var result = string.Join(Environment.NewLine, authors.Select(c => $"{c.Name} - {c.Copies}"));
            return result;
        }

        //12.	Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var profitByCategory = context.Categories
                                          .Select(c => new
                                          {
                                              c.Name,
                                              Profit = c.CategoryBooks.Select(cb => cb.Book.Copies * cb.Book.Price).Sum()
                                          })
                                          .OrderByDescending(c => c.Profit)
                                          .ThenBy(c => c.Name)
                                          .ToList();


            var result = string.Join(Environment.NewLine, profitByCategory.Select(p => $"{p.Name} ${p.Profit:F2}"));
            return result;
        }

        //13.	Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var booksByCategory = context.Categories
                              .OrderBy(c => c.Name)
                              .Select(c => new
                              {
                                  c.Name,
                                  book = c.CategoryBooks
                                          .Select(cb => cb.Book)
                                          .OrderByDescending(cb => cb.ReleaseDate)
                                          .Take(3)
                                                        
                              })
                              .ToList();

            var result = string.Join(Environment.NewLine, booksByCategory.Select(c => $"--{c.Name}" +
            $"{Environment.NewLine}" +
            $"{String.Join(Environment.NewLine, c.book.Select(b => $"{b.Title} ({b.ReleaseDate.Value.Year})"))}"
            ));

            return result;
        }

        //14.	Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            context.Books
                   .Where(b => b.ReleaseDate.Value.Year < 2010)
                   .ToList()
                   .ForEach(b => b.Price += 5);

            context.SaveChanges();
        }

        //15.	Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            var booksForDel = context.Books
                                     .Where(b => b.Copies < 4200)
                                     .ToList();

            context.RemoveRange(booksForDel);
            context.SaveChanges();

            return booksForDel.Count();
        }
    }
}

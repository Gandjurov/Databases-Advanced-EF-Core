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

                var result = GetBooksByAgeRestriction(db, "teEN");

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
    }
}

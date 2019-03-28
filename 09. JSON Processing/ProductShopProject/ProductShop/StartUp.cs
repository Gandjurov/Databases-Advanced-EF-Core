namespace ProductShop
{
    using Newtonsoft.Json;
    using ProductShop.Data;
    using ProductShop.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            var context = new ProductShopContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var usersJson = File.ReadAllText(@"D:\SoftwareUniversity\#GitRepositories\Databases-Advanced-EF-Core\09. JSON Processing\ProductShopProject\ProductShop\Datasets\users.json");

            var result = ImportUsers(context, usersJson);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson)
                                      .Where(u => u.LastName != null && u.LastName.Length >= 3)
                                      .ToList();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";

        }
    }
}
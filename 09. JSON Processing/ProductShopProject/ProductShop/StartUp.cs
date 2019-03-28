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

            var usersJson = File.ReadAllText(@"D:\SoftwareUniversity\#GitRepositories\Databases-Advanced-EF-Core\09. JSON Processing\ProductShopProject\ProductShop\Datasets\users.json");
            var productsJson = File.ReadAllText(@"D:\SoftwareUniversity\#GitRepositories\Databases-Advanced-EF-Core\09. JSON Processing\ProductShopProject\ProductShop\Datasets\products.json");

            Console.WriteLine(ImportProducts(context, productsJson));
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

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson)
                                      .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Length >= 3)
                                      .ToList();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
    }
}
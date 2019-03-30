namespace ProductShop
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProductShop.Data;
    using ProductShop.Dtos.Export;
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
            

            var usersJson = File.ReadAllText(@"..\..\..\Datasets\users.json");
            var productsJson = File.ReadAllText(@"..\..\..\Datasets\products.json");
            var categoriesJson = File.ReadAllText(@"D..\..\..\Datasets\categories.json");
            var categoryProductsJson = File.ReadAllText(@"..\..\..\Datasets\categories-products.json");

            //Console.WriteLine(ImportUsers(context, usersJson));
            //Console.WriteLine(ImportProducts(context, productsJson));
            //Console.WriteLine(ImportCategories(context, categoriesJson));
            //Console.WriteLine(ImportCategories(context, categoryProductsJson));

            //Console.WriteLine(GetProductsInRange(context));
            //Console.WriteLine(GetSoldProducts(context));
            Console.WriteLine(GetCategoriesByProductsCount(context));
            //Console.WriteLine(GetUsersWithProducts(context));
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

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                          .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Length >= 3 && c.Name.Length <= 15)
                          .ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);

            var validCategoryIds = context.Categories
                                          .Select(c => c.Id)
                                          .ToHashSet();

            var validProductIds = context.Products
                                          .Select(p => p.Id)
                                          .ToHashSet();

            var validEntites = new List<CategoryProduct>();

            foreach (var categoryProduct in categoryProducts)
            {
                bool isValid = validCategoryIds.Contains(categoryProduct.CategoryId) &&
                               validProductIds.Contains(categoryProduct.ProductId);

                if (isValid)
                {
                    validEntites.Add(categoryProduct);
                }
            }

            context.CategoryProducts.AddRange(validEntites);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";

        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                   .Where(p => p.Price >= 500 && p.Price <= 1000)
                   .Select(p => new ProductDto
                   {
                       Name = p.Name,
                       Price = p.Price,
                       Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                   })
                   .OrderBy(p => p.Price)
                   .ToList();

            var json = JsonConvert.SerializeObject(productsInRange, Formatting.Indented);

            return json;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var filteredUsers = context.Users
                   .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                   .OrderBy(u => u.LastName)
                   .ThenBy(u => u.FirstName)
                   .Select(u => new
                   {
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       SoldProducts = u.ProductsSold
                            .Where(ps => ps.Buyer != null)
                            .Select(ps => new
                            {
                                Name = ps.Name,
                                Price = ps.Price,
                                BuyerFirstName = ps.Buyer.FirstName,
                                BuyerLastName = ps.Buyer.LastName
                            }).ToList()
                   })
                   .ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(
                filteredUsers,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver
                });

            return json;

        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Average(c => c.Product.Price):F2}",
                    TotalRevenue = $"{x.CategoryProducts.Sum(c => c.Product.Price)}"
                })
                .ToList();

            string json = JsonConvert.SerializeObject(categories,
                new JsonSerializerSettings()
                {
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(),
                    },

                    Formatting = Formatting.Indented
                }
            );

            return json;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var filteredUsers = context.Users
                                       .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                                       .OrderByDescending(u => u.ProductsSold.Count(ps => ps.Buyer != null))
                                       .Select(u => new
                                       {
                                           FirstName = u.FirstName,
                                           LastName = u.LastName,
                                           Age = u.Age,
                                           SoldProducts = new
                                           {
                                               Count = u.ProductsSold
                                                        .Count(ps => ps.Buyer != null),
                                               Products = u.ProductsSold
                                                           .Where(ps => ps.Buyer != null)
                                                           .Select(ps => new
                                                           {
                                                               Name = ps.Name,
                                                               Price = ps.Price
                                                           })
                                                           .ToList()
                                           }
                                       })
                                       .ToList();

            var result = new
            {
                UsersCount = filteredUsers.Count,
                Users = filteredUsers
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(
                result,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = contractResolver,
                    NullValueHandling = NullValueHandling.Ignore
                });

            return json;
        }
    }
}
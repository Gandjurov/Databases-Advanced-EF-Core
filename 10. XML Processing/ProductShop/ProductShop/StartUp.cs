using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            Mapper.Initialize(x => 
            {
                x.AddProfile<ProductShopProfile>();
            });

            var usersXml = File.ReadAllText("../../../Datasets/users.xml");
            var productsXml = File.ReadAllText("../../../Datasets/Products.xml");

            using (ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //01. Import Users 
                //var usersResult = ImportUsers(context, usersXml);
                var productsResult = ImportProducts(context, productsXml);

                //Console.WriteLine(usersResult);
                Console.WriteLine(productsResult);
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var productDto in productsDto)
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    SellerId = productDto.SellerId,
                    BuyerId = productDto.BuyerId
                };

                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(new CarDealerProfile()));
            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var suppliersJson = File.ReadAllText(@"..\..\..\Datasets\suppliers.json");
            var partsJson = File.ReadAllText(@"..\..\..\Datasets\parts.json");
            var carsJson = File.ReadAllText(@"..\..\..\Datasets\cars.json");
            var customersJson = File.ReadAllText(@"..\..\..\Datasets\customers.json");
            var salesJson = File.ReadAllText(@"..\..\..\Datasets\sales.json");

            //Console.WriteLine(ImportSuppliers(context, suppliersJson));
            //Console.WriteLine(ImportParts(context, partsJson));
            //Console.WriteLine(ImportCars(context, carsJson));
            //Console.WriteLine(ImportCustomers(context, customersJson));
            //Console.WriteLine(ImportSales(context, salesJson));

            //Console.WriteLine(GetOrderedCustomers(context));
            //Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(inputJson)
                                       .Where(s => !string.IsNullOrEmpty(s.Name))
                                       .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliers = context.Suppliers
                                   .ToHashSet();

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)
                           .ToHashSet();

            List<Part> validParts = new List<Part>();

            foreach (var part in parts)
            {
                foreach (var supplier in suppliers)
                {
                    if (part.SupplierId == supplier.Id)
                    {
                        validParts.Add(part);
                        context.Parts.Add(part);
                    }
                }
            }


            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<CarInsertDto[]>(inputJson);
            var mappedCars = new List<Car>();

            foreach (var car in cars)
            {
                Car vehicle = Mapper.Map<CarInsertDto, Car>(car);
                mappedCars.Add(vehicle);

                var partIds = car.PartsId
                                 .Distinct()
                                 .ToList();

                if (partIds == null)
                {
                    continue;
                }

                partIds.ForEach(pid =>
                {
                    var currentPair = new PartCar()
                    {
                        Car = vehicle,
                        PartId = pid
                    };

                    vehicle.PartCars.Add(currentPair);
                });


            }

            context.Cars.AddRange(mappedCars);
            context.SaveChanges();
            int affectedRows = context.Cars.Count();

            return $"Successfully imported {affectedRows}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson)
                            .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson)
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                                   .OrderBy(c => c.BirthDate)
                                   .ThenBy(c => c.IsYoungDriver)
                                   .ToList();

            string json = JsonConvert.SerializeObject(customers, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,

                DateFormatString = "dd/MM/yyyy",
                Formatting = Formatting.Indented,
                //ContractResolver = new DefaultContractResolver()
                //{
                //    NamingStrategy = new CamelCaseNamingStrategy()
                //}
            });

            return json;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                              .Where(c => string.Compare(c.Make, "Toyota", true) == 0)
                              .OrderBy(c => c.Model)
                              .ThenByDescending(c => c.TravelledDistance)
                              .ToList();

            string json = JsonConvert.SerializeObject(cars, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                
                Formatting = Formatting.Indented,
                //ContractResolver = new DefaultContractResolver()
                //{
                //    NamingStrategy = new CamelCaseNamingStrategy()
                //}
            });

            return json;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                                   .Where(s => s.IsImporter == false)
                                   .Select(s => new
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       PartsCount = s.Parts.Count
                                   })
                                   .ToList();

            var json = JsonConvert.SerializeObject(suppliers, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                              .Include(c => c.PartCars)
                              .ThenInclude(c => c.Part)
                              .Select(c => new
                              {
                                  car = new
                                  {
                                      Make = c.Make,
                                      Model = c.Model,
                                      TravelledDistance = c.TravelledDistance
                                  },

                                  parts = c.PartCars
                                           .Select(p => new
                                           {
                                               Name = p.Part.Name,
                                               Price = $"{p.Part.Price:F2}"
                                           })
                                           .ToList()
                                    
                              })
                              .ToList();

            var json = JsonConvert.SerializeObject(cars, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                                   .Include(c => c.Sales)
                                   .ThenInclude(s => s.Car)
                                   .ThenInclude(c => c.PartCars)
                                   .ThenInclude(pc => pc.Part)
                                   .Where(c => c.Sales.Count >= 1)
                                   .Select(x => new
                                   {
                                       FullName = x.Name,
                                       BoughtCars = x.Sales.Count(),
                                       SpentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                                   })
                                   .OrderByDescending(a => a.SpentMoney)
                                   .ThenBy(a => a.BoughtCars)
                                   .ToList();


            var json = JsonConvert.SerializeObject(customers, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                               .Take(10)
                               .Select(x => new
                               {
                                   car = new
                                   {
                                       Make = x.Car.Make,
                                       Model = x.Car.Model,
                                       TravelledDistance = x.Car.TravelledDistance
                                   },

                                   customerName = x.Customer.Name,
                                   Discount = $"{x.Discount:F2}",
                                   price = $"{x.Car.PartCars.Sum(y => y.Part.Price):F2}",
                                   priceWithDiscount = $"{x.Car.PartCars.Sum(y => y.Part.Price) - (x.Car.PartCars.Sum(y => y.Part.Price) * (x.Discount / 100)):F2}"
                               })
                               .ToList();


            var json = JsonConvert.SerializeObject(sales, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                //ContractResolver = new DefaultContractResolver()
                //{
                //    NamingStrategy = new CamelCaseNamingStrategy()
                //}
            });

            return json;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

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
            Console.WriteLine(ImportCustomers(context, customersJson));
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
    }
}
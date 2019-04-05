using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });

            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            var customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            var salesXml = File.ReadAllText("../../../Datasets/sales.xml");

            using (CarDealerContext context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Imports:
                //var importSuppliers = ImportSuppliers(context, suppliersXml);
                //var importParts = ImportParts(context, partsXml);
                //var importCars = ImportCars(context, carsXml);
                var importCustomers = ImportCustomers(context, customersXml);
                //Exports:


                Console.WriteLine(importCustomers);
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSuppliersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var supplier = new Supplier
                {
                    Name = supplierDto.Name,
                    IsImporter = supplierDto.IsImporter
                };

                suppliers.Add(supplier);
            };

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartsDto[]), new XmlRootAttribute("Parts"));

            var partsDto = (ImportPartsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();
            var supplierIds = context.Suppliers
                                     .Select(x => x.Id)
                                     .ToList();

            foreach (var partDto in partsDto)
            {
                var part = new Part
                {
                    Name = partDto.Name,
                    Price = partDto.Price,
                    Quantity = partDto.Quantity,
                    SupplierId = partDto.SupplierId
                };

                if (supplierIds.Contains(part.SupplierId) == false)
                {
                    continue;
                }

                parts.Add(part);
            };

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarsDto[]), new XmlRootAttribute("Cars"));

            var carsDto = (ImportCarsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var mappedCars = new List<Car>();

            foreach (var carDto in carsDto)
            {
                var vehicle = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };

                mappedCars.Add(vehicle);

                var partIds = carDto.Parts
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
                        PartId = pid.partId
                    };

                    vehicle.PartCars.Add(currentPair);
                });


            }

            context.Cars.AddRange(mappedCars);
            context.SaveChanges();
            int affectedRows = context.Cars.Count();

            return $"Successfully imported {affectedRows}.";


            //var cars = new List<Car>();
            //var partIds = new HashSet<Car>();
            //var existingPartsIds = context.Parts
            //                              .Select(x => x.Id)
            //                              .ToArray();

            //foreach (var carDto in carsDto)
            //{
            //    var car = new Car
            //    {
            //        Make = carDto.Make,
            //        Model = carDto.Model,
            //        TraveledDistance = carDto.TraveledDistance
            //    };

            //    foreach (var part in carDto.Parts)
            //    {
            //        var pid = part.partId;

            //        if (existingPartsIds.Contains(pid) == false)
            //        {
            //            continue;
            //        }

            //        PartCar currentPair = new PartCar()
            //        {
            //            Car = car,
            //            PartId = pid
            //        };

            //        car.PartCars.Add(currentPair);


            //    }

            //    cars.Add(car);
            //}

            //context.Cars.AddRange(cars);
            //context.SaveChanges();

            //return $"Successfully imported {cars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomersDto[]), new XmlRootAttribute("Customers"));

            var customersDto = (ImportCustomersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customerDto in customersDto)
            {
                var customer = new Customer
                {
                    Name = customerDto.Name,
                    BirthDate = customerDto.BirthDate,
                    IsYoungDriver = customerDto.IsYoungDriver
                };
                
                customers.Add(customer);
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }
    }
}
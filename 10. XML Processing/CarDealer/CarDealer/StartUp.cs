using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
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
                //var importCustomers = ImportCustomers(context, customersXml);
                //var importSales = ImportSales(context, salesXml);


                //Exports:
                //var exportCarWithDistance = GetCarsWithDistance(context);
                //var exportCarWithDistance = GetCarsFromMakeBmw(context);
                //var exportLocalSuppliers = GetLocalSuppliers(context);
                //var exportCarsWithTheirListOfParts = GetCarsWithTheirListOfParts(context);
                var exportTotalSalesByCustomer = GetTotalSalesByCustomer(context);

                Console.WriteLine(exportTotalSalesByCustomer);
            }
        }

        //Imports:
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

            var cars = new List<Car>();

            var existingPartsIds = context.Parts
                                          .Select(x => x.Id)
                                          .Distinct()
                                          .ToArray();

            foreach (var car in carsDto)
            {
                Car currentCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TraveledDistance = car.TraveledDistance

                };

                var partIds = new HashSet<int>();

                foreach (var id in car.Parts)
                {
                    var pid = id.Id;
                    partIds.Add(pid);
                }

                foreach (var pid in partIds)
                {
                    if (existingPartsIds.Contains(pid) == false)
                        continue;

                    PartCar currentPair = new PartCar()
                    {
                        Car = currentCar,
                        PartId = pid
                    };

                    currentCar.PartCars.Add(currentPair);
                }

                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
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

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), new XmlRootAttribute("Sales"));

            var salesDto = (ImportSalesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));
            var sales = new List<Sale>();

            var existingCarIds = context.Cars
                                .Select(x => x.Id)
                                .ToArray();

            var existingCustomerIds = context.Customers
                                     .Select(x => x.Id)
                                     .ToArray();

            foreach (var saleDto in salesDto)
            {
                var currentCarId = saleDto.CarId;
                var currentCustomerId = saleDto.CustomerId;

                if (!existingCarIds.Contains(currentCarId))
                {
                    continue;
                }

                var sale = new Sale
                {
                    CarId = currentCarId,
                    CustomerId = currentCustomerId,
                    Discount = saleDto.Discount
                };
                sales.Add(sale);
            };

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //Exports:
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                              .Where(c => c.TraveledDistance >= 2000000)
                              .Select(x => new ExportCarWithDistanceDto
                              {
                                  Make = x.Make,
                                  Model = x.Model,
                                  TraveledDistance = x.TraveledDistance
                              })
                              .OrderBy(m => m.Make)
                              .ThenBy(m => m.Model)
                              .Take(10)
                              .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarWithDistanceDto[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                              .Where(c => c.Make == "BMW")
                              .Select(x => new ExportBMWCarsDto
                              {
                                  Id = x.Id,
                                  Model = x.Model,
                                  TraveledDistance = x.TraveledDistance
                              })
                              .OrderBy(c => c.Model)
                              .ThenByDescending(c => c.TraveledDistance)
                              .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportBMWCarsDto>), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                                   .Where(s => s.IsImporter == false)
                                   .Select(x => new ExportSupplierDto
                                   {
                                       Id = x.Id,
                                       Name = x.Name,
                                       PartsCount = x.Parts.Count()
                                   })
                                   .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportSupplierDto>), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("","")
            });

            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                              .Select(x => new ExportCarsWithTheirListOfPartsDto
                              {
                                  Make = x.Make,
                                  Model = x.Model,
                                  TraveledDistance = x.TraveledDistance,
                                  Parts = x.PartCars.Select(p => new ExportPartsDto()
                                  {
                                      Name = p.Part.Name,
                                      Price = p.Part.Price
                                  })
                                  .OrderByDescending(p => p.Price)
                                  .ToList()
                              })
                              .OrderByDescending(c => c.TraveledDistance)
                              .ThenBy(c => c.Model)
                              .Take(5)
                              .ToList();
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportCarsWithTheirListOfPartsDto>), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                    new XmlQualifiedName("","")
                });

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                                   .Where(x => x.Sales.Count() >= 1)
                                   .Select(x => new ExportTotalSalesByCustomerDto
                                   {
                                       Name = x.Name,
                                       BoughtCars = x.Sales.Count(),
                                       SpentMoney = x.Sales.Sum(y => y.Car.PartCars.Sum(z => z.Part.Price))
                                   })
                                   .OrderByDescending(x => x.SpentMoney)
                                   .ToList();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExportTotalSalesByCustomerDto>), new XmlRootAttribute("customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                    new XmlQualifiedName("","")
                });

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
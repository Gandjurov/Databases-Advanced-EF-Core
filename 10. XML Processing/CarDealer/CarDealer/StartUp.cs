using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                //Imports:
                var importSuppliers = ImportSuppliers(context, suppliersXml);


                //Exports:

                Console.WriteLine(importSuppliers);
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
    }
}
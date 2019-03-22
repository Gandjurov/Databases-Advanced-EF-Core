using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetAddressCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public SetAddressCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            string address = string.Join(" ", inputArgs.Skip(1));

            var employee = context.Employees.Find(employeeId);

            employee.Address = address;

            context.SaveChanges();

            return $"{employee.FirstName} {employee.LastName}'s address is set to {address}";
        }
    }
}

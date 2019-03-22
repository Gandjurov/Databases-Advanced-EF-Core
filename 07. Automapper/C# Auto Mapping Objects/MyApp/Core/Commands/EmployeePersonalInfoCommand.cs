using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfoCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public EmployeePersonalInfoCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);

            var employee = context.Employees.Find(employeeId);

            string birthDate = employee.Birthday != null ? employee.Birthday.Value.ToString() : "[no data]";
            string address = employee.Address != null ? employee.Address : "[no data]";

            return $"ID: {employee.Id} - {employee.FirstName} {employee.LastName} - ${employee.Salary:f2}" + 
                Environment.NewLine + 
                $"Birthday: {birthDate}" + 
                Environment.NewLine + 
                $"Address: {address}";
        }
    }
}

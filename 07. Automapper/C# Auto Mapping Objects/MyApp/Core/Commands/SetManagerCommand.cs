using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetManagerCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public SetManagerCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            int managerId = int.Parse(inputArgs[1]);

            //TODO check for null
            var employee = this.context.Employees.Find(employeeId);
            var manager = this.context.Employees.Find(managerId);

            employee.Manager = manager;
            this.context.SaveChanges();

            return "Setting manager completed successfully";
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Commands.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ListEmployeesOlderThanCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ListEmployeesOlderThanCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            StringBuilder sb = new StringBuilder();

            int age = int.Parse(inputArgs[0]);

            var employees = context.Employees
                                   .Include(e => e.Manager)
                                   .Where(e => e.Birthday != null && e.Birthday.Value.Year + age < DateTime.Now.Year)
                                   .OrderByDescending(e => e.Salary)
                                   .Select(e => e.Manager == null
                                        ? $"{e.FirstName} {e.LastName} - ${e.Salary:F2} - Manager: [no manager]"
                                        : $"{e.FirstName} {e.LastName} - ${e.Salary:F2} - Manager: {e.Manager.LastName}")
                                   .ToArray();
            
            foreach (var textRow in employees)
            {
                sb.AppendLine(textRow);
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}

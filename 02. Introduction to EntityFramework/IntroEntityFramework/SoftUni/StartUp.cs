using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            using (var context = new SoftUniContext())
            {
                var result = GetAddressesByTown(context);
                Console.WriteLine(result);
            }
        }

        // TASK - 03. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.LastName,
                                       e.MiddleName,
                                       e.JobTitle,
                                       e.Salary,
                                       e.EmployeeId
                                   })
                                   .OrderBy(x => x.EmployeeId)
                                   .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} " +
                    $"{employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().Trim();
        }

        // TASK - 04. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   .Select(e => new
                                   {
                                       e.FirstName,
                                       e.Salary
                                   })
                                   .Where(e => e.Salary > 50000)
                                   .OrderBy(x => x.FirstName)
                                   .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().Trim();
        }

        //TASK - 05. Employees from Research and Development 
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   .Where(e => e.Department.Name == "Research and Development")
                                   .Select(e => new
                                        {
                                           e.FirstName,
                                           e.LastName,
                                           Department = e.Department.Name,
                                           e.Salary
                                        })
                                   .OrderBy(x => x.Salary)
                                   .ThenByDescending(x => x.FirstName)
                                   .ToList();


            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from Research and Development - ${employee.Salary:F2}");
            }

            return sb.ToString().Trim();
        }

        //TASK - 06. Adding a New Address and Updating Employee 
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            //context.Addresses.Add(address);

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            nakov.Address = address;

            context.SaveChanges();

            var employeeAddresses = context.Employees
                                           .OrderByDescending(x => x.AddressId)
                                           .Select(a => a.Address.AddressText)
                                           .Take(10)
                                           .ToList();

            foreach (var employeeAddress in employeeAddresses)
            {
                sb.AppendLine($"{employeeAddress}");
            }


            return sb.ToString().Trim();
        }

        //TASK - 07. Employees and Projects 
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   .Where(p => p.EmployeesProjects.Any(s => s.Project.StartDate.Year >= 2001 && s.Project.StartDate.Year <= 2003))
                                   .Select(e => new
                                   {
                                       EmployeeFullName = e.FirstName + " " + e.LastName,
                                       ManagerFullName = e.Manager.FirstName + " " + e.Manager.LastName,
                                       Projects = e.EmployeesProjects.Select(p => new
                                       {
                                           ProjectName = p.Project.Name,
                                           StartDate = p.Project.StartDate,
                                           EndDate = p.Project.EndDate
                                       }).ToList()
                                   })
                                   .Take(10)
                                   .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.EmployeeFullName} - Manager: {employee.ManagerFullName}");

                foreach (var project in employee.Projects)
                {
                    var startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    var endDate = project.EndDate.HasValue ? project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                                  : "not finished";
                    
                    sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
                }
            }

            return sb.ToString().Trim();
        }

        //TASK - 08. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context.Addresses
                                   .OrderByDescending(a => a.Employees.Count)
                                   .ThenBy(a => a.Town.Name)
                                   .Take(10)
                                   .Select(a => new
                                   {
                                       AddressText = a.AddressText,
                                       Town = a.Town.Name,
                                       EmployeesCount = a.Employees.Count
                                   })
                                   .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.Town} - {address.EmployeesCount} employees");
            }

            return sb.ToString().Trim();
        }

        
    }
}

using Microsoft.EntityFrameworkCore;
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
                var result = RemoveTown(context);
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

        //TASK - 09. Employee 147 
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employee = context.Employees
                                  .Where(e => e.EmployeeId == 147)
                                  .Select(e => new
                                  {
                                      e.FirstName,
                                      e.LastName,
                                      e.JobTitle,
                                      projects = e.EmployeesProjects
                                        .Select(ep => ep.Project.Name)
                                        .OrderBy(p => p)
                                        .ToList()
                                  })
                                  .First();

            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

            foreach (var project in employee.projects)
            {
                sb.AppendLine($"{project}");
            }

            return sb.ToString().Trim();
        }

        //TASK - 10. Departments with More Than 5 Employees 
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                                     .Where(e => e.Employees.Count > 5)
                                     .OrderBy(e => e.Employees.Count)
                                     .ThenBy(d => d.Name)
                                     .Select(x => new
                                     {
                                         DepartmentName = x.Name,
                                         ManagerFullName = x.Manager.FirstName + " " + x.Manager.LastName,
                                         Employees = x.Employees.Select(e => new
                                         {
                                             EmployeeFullName = e.FirstName + " " + e.LastName,
                                             JobTitle = e.JobTitle
                                         }).OrderBy(f => f.EmployeeFullName)
                                           .ToList()
                                     }).ToList();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepartmentName} – {department.ManagerFullName}");

                foreach (var employee in department.Employees)
                {
                    sb.AppendLine($"{employee.EmployeeFullName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        //TASK - 11. Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
                                .OrderByDescending(p => p.StartDate)
                                .Take(10)
                                .Select(p => new
                                            {
                                                p.Name,
                                                p.Description,
                                                p.StartDate
                                            })
                                .OrderBy(p => p.Name)
                                .ToList();

            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}{Environment.NewLine}{project.Description}{Environment.NewLine}{project.StartDate}");
            }

            return sb.ToString().Trim();
        }

        //TASK - 12. Increase Salaries 
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            context.Employees
                   .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }.Contains(e.Department.Name))
                   .ToList()
                   .ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            var employees = context.Employees
                                   .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }.Contains(e.Department.Name))
                                   .OrderBy(e => e.FirstName)
                                   .ThenBy(e => e.LastName)
                                   .ToList();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} (${emp.Salary:F2})");
            }

            return sb.ToString().Trim();
        }

        //TASK - 13. Find Employees by First Name Starting With "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                                   //.Where(f => f.FirstName.StartsWith("Sa"))
                                   .Where(f => EF.Functions.Like(f.FirstName, "sa%"))
                                   .Select(x => new
                                   {
                                       x.FirstName,
                                       x.LastName,
                                       x.JobTitle,
                                       x.Salary
                                   })
                                   .OrderBy(f => f.FirstName)
                                   .ThenBy(f => f.LastName)
                                   .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F2})");
            }


            return sb.ToString().Trim();
        }


        //TASK - 14. Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var project = context.Projects.FirstOrDefault(x => x.ProjectId == 2);

            var employeeProjects = context.EmployeesProjects.Where(x => x.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(employeeProjects);

            //foreach (var employeeProject in employeeProjects)
            //{
            //    context.EmployeesProjects.Remove(employeeProject);
            //}

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                                  .Select(x => x.Name)
                                  .Take(10)
                                  .ToList();

            foreach (var currentProject in projects)
            {
                sb.AppendLine(currentProject);
            }

            return sb.ToString().Trim();
        }

        //TASK - 15. Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            string townName = "Seattle";

            context.Employees
                .Where(e => e.Address.Town.Name == townName)
                .ToList()
                .ForEach(e => e.AddressId = null);

            int addressesCount = context.Addresses
                .Where(a => a.Town.Name == townName)
                .Count();

            context.Addresses
                .Where(a => a.Town.Name == townName)
                .ToList()
                .ForEach(a => context.Addresses.Remove(a));

            context.Towns
                .Remove(context.Towns
                    .SingleOrDefault(t => t.Name == townName));

            context.SaveChanges();

            sb.AppendLine($"{addressesCount} {(addressesCount == 1 ? "address" : "addresses")} in {townName} {(addressesCount == 1 ? "was" : "were")} deleted");

            return sb.ToString().Trim();
        }
    }
}

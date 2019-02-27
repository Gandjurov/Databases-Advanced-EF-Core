using SoftUni.Data;
using System;
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
                var result = GetEmployeesFullInformation(context);
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
    }
}

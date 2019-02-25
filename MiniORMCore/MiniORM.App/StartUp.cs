namespace MiniORM.App
{
    using MiniORM.App.Data;
    using MiniORM.App.Data.Entities;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            const string connectionString = @"Server=(local)\SQLEXPRESS;DataBase=MiniORM;Integrated Security=True";

            var context = new SoftUniDbContext(connectionString);

            //var emploees = context.Employees.ToList();

            var newEmployee = new Employee
            {
                FirstName = "Pesho",
                LastName = "Petrov",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            };

            context.Employees.Add(newEmployee);
            context.SaveChanges();

            var secondEmployee = new Employee
            {
                FirstName = "Gosho",
                LastName = "Georgiev",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true
            };
            context.Employees.Add(secondEmployee);
            context.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;

namespace BillsPaymentSystem.App
{
    public class DbInitilizer
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            SeedUsers(context);
        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = { "Gosho", "Pesho", "Ivan", "Kiro", null, "" };
            string[] lastNames = { "Goshev", "Peshev", "Ivanov", "Kirov", null, "ERROR" };
            string[] emails = { "Gosho@abv.bg", "Pesho@abv.bg", "Ivan@abv.bg", "Kiro@abv.bg", null, "ERROR" };
            string[] passwords = { "sdgsgsg", "qtqetcbxcb", "derthjuerghwr", "xfheruerb", null, "ERROR" };

            List<User> users = new List<User>();

            for (int i = 0; i < firstNames.Length; i++)
            {
                var user = new User
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i],
                    Password = passwords[i]
                };

                users.Add(user);

                if (!IsValid(user))
                {
                    continue;
                }

            }

            context.Users.AddRange(users);
            context.SaveChanges();

        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}

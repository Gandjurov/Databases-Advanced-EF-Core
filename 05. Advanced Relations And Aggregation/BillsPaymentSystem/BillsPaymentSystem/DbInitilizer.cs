using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using BillsPaymentSystem.Models.Enums;

namespace BillsPaymentSystem.App
{
    public class DbInitilizer
    {
        public static void Seed(BillsPaymentSystemContext context)
        {
            SeedUsers(context);
            SeedCreditCards(context);
            SeedBankAccounts(context);
            SeedPaymentMethods(context);
        }

        private static void SeedPaymentMethods(BillsPaymentSystemContext context)
        {
            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

            for (int i = 0; i < 8; i++)
            {
                var paymentMethod = new PaymentMethod()
                {
                    UserId = new Random().Next(1, 5),
                    Type = (PaymentType)new Random().Next(0, 2),

                };


                if (i % 3 == 0)
                {
                    paymentMethod.CreditCardId = new Random().Next(1, 5);
                    paymentMethod.BankAccountId = new Random().Next(1, 5);
                }
                else if (i % 2 == 0)
                {
                    paymentMethod.CreditCardId = new Random().Next(1, 5);
                }
                else
                {
                    paymentMethod.BankAccountId = new Random().Next(1, 5);
                }

                if (!IsValid(paymentMethod))
                {
                    continue;
                }
                

                paymentMethods.Add(paymentMethod);
            }

            context.PaymentMethods.AddRange(paymentMethods);
            context.SaveChanges();
        }

        private static void SeedBankAccounts(BillsPaymentSystemContext context)
        {
            List<BankAccount> bankAccounts = new List<BankAccount>();

            for (int i = 0; i < 8; i++)
            {
                var bankAccount = new BankAccount()
                {
                    Balance = new Random().Next(-200, 200),
                    BankName = "Banka" + i,
                    SWIFT = "Swift" + i + 1
                };

                if (!IsValid(bankAccount))
                {
                    continue;
                }

                bankAccounts.Add(bankAccount);
            }

            context.BankAccounts.AddRange(bankAccounts);
            context.SaveChanges();

        }

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            var creditCards = new List<CreditCard>();

            for (int i = 0; i < 8; i++)
            {
                var creditCard = new CreditCard()
                {
                    Limit = new Random().Next(-25000, 25000),
                    MoneyOwed = new Random().Next(-25000, 25000),
                    ExpirationDate = DateTime.Now.AddDays(new Random().Next(-200, 200))
                };

                if (!IsValid(creditCard))
                {
                    continue;
                }

                creditCards.Add(creditCard);
                
            }

            context.CreditCards.AddRange(creditCards);
            context.SaveChanges();
        }

        private static void SeedUsers(BillsPaymentSystemContext context)
        {
            string[] firstNames = { "Gosho", "Pesho", "Ivan", "Kiro", null, "" };
            string[] lastNames = { "Goshev", "Peshev", "Ivanov", "Kirov", null, "" };
            string[] emails = { "Gosho@abv.bg", "Pesho@abv.bg", "Ivan@abv.bg", "Kiro@abv.bg", null, "ERROR" };
            string[] passwords = { "sdgsgsg", "qtqetcbxcb", "derthjuerghwr", "xfheruerb", null, "ERROR" };

            List<User> users = new List<User>();

            for (int i = 0; i < firstNames.Length; i++)
            {
                var user = new User()
                {
                    FirstName = firstNames[i],
                    LastName = lastNames[i],
                    Email = emails[i],
                    Password = passwords[i]
                };

                
                if (!IsValid(user))
                {
                    continue;
                }

                users.Add(user);
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

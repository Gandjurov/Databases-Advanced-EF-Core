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
            //SeedUsers(context);
            //SeedCreditCards(context);
            //SeedBankAccounts(context);
            SeedPaymentMethods(context);
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

        private static void SeedCreditCards(BillsPaymentSystemContext context)
        {
            decimal[] limits = { 15000.00m, 20000m, 15000m, 16000m };
            decimal[] moneysOwed = { 1500.00m, 1800m, 14000m, 4500m };
            DateTime[] ExpirationDates =
            {
                new DateTime(2018, 6, 20),
                new DateTime(2018, 6, 25),
                new DateTime(2019, 7, 4),
                new DateTime(2019, 2, 5)
            };

            var creditCards = new List<CreditCard>();

            for (int i = 0; i < limits.Length; i++)
            {
                var creditCard = new CreditCard()
                {
                    Limit = limits[i],
                    MoneyOwed = moneysOwed[i],
                    ExpirationDate = ExpirationDates[i]
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

        private static void SeedBankAccounts(BillsPaymentSystemContext context)
        {
            decimal[] balances = { 2455m, 12000m, 14000m, 8500m };
            string[] bankNames = { "SG Expresbank", "Investbank", "DSK", "Raiffensen bank" };
            string[] swifts = { "TGBHJKL", "TBGINKFL", "TBGDSK", "TBGFRF" };

            List<BankAccount> bankAccounts = new List<BankAccount>();

            for (int i = 0; i < balances.Length; i++)
            {
                var bankAccount = new BankAccount()
                {
                    Balance = balances[i],
                    BankName = bankNames[i],
                    SWIFT = swifts[i]
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

        private static void SeedPaymentMethods(BillsPaymentSystemContext context)
        {
            PaymentType[] types =
                {
                PaymentType.CreditCard,
                PaymentType.BankAccount,
                PaymentType.CreditCard,
                PaymentType.BankAccount
            };


            List<PaymentMethod> paymentMethods = new List<PaymentMethod>();

            for (int i = 0; i < types.Length; i++)
            {
                var paymentMethod = new PaymentMethod()
                {
                    UserId = i,
                    Type = types[i],
                };
                paymentMethod.CreditCardId = new Random().Next(1, 2);
                paymentMethod.BankAccountId = new Random().Next(1, 2);


                if (!IsValid(paymentMethod))
                {
                    continue;
                }

                paymentMethods.Add(paymentMethod);
            }
            context.PaymentMethods.AddRange(paymentMethods);
            context.SaveChanges();

            //if (i % 3 == 0)
            //{
            //    paymentMethod.CreditCardId = new Random().Next(1, 5);
            //    paymentMethod.BankAccountId = new Random().Next(1, 5);
            //}
            //else if (i % 2 == 0)
            //{
            //    paymentMethod.CreditCardId = new Random().Next(1, 5);
            //}
            //else
            //{
            //    paymentMethod.BankAccountId = new Random().Next(1, 5);
            //}


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


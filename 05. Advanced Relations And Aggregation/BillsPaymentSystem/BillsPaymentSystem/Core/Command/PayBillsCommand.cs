using BillsPaymentSystem.App.Core.Command.Contracts;
using BillsPaymentSystem.Data;
using BillsPaymentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Command
{
    public class PayBillsCommand : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public PayBillsCommand(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int userId = int.Parse(args[0]);
            decimal amount = decimal.Parse(args[1]);

            var user = this.context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                throw new ArgumentNullException("User is not found!");
            }

            PayBills(user, amount);

            return "";
        }

        private void PayBills(User user, decimal amount)
        {
            var bankAccountTotals = user.PaymentMethods
                            .Where(x => x.BankAccount != null).Sum(x => x.BankAccount.Balance);

            var CreditCardTotals = user.PaymentMethods
                            .Where(x => x.CreditAccount != null).Sum(x => x.CreditAccount.LimitLeft);

            var totalAmount = bankAccountTotals + CreditCardTotals;

            if (totalAmount >= amount)
            {
                var bankAccounts = user.PaymentMethods
                                       .Where(x => x.BankAccount != null)
                                       .Select(x => x.BankAccount)
                                       .OrderBy(x => x.BankAccountId);

                foreach (var bankAccount in bankAccounts)
                {
                    if (bankAccount.Balance >= amount)
                    {
                        bankAccount.WithDraw(amount);
                        amount = 0;
                    }
                    else
                    {
                        amount -= bankAccount.Balance;
                        bankAccount.WithDraw(bankAccount.Balance);
                    }

                    if (amount == 0)
                    {
                        return;
                    }
                }

                var creditCards = user.PaymentMethods
                       .Where(x => x.CreditAccount != null)
                       .Select(x => x.CreditAccount)
                       .OrderBy(x => x.CreditCardId);

                foreach (var creditCard in creditCards)
                {
                    if (creditCard.LimitLeft >= amount)
                    {
                        creditCard.WithDraw(amount);
                        amount = 0;
                    }
                    else
                    {
                        amount -= creditCard.LimitLeft;
                        creditCard.WithDraw(creditCard.LimitLeft);
                    }

                    if (amount == 0)
                    {
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Insufficient funds!");
            }
        }
    }
}

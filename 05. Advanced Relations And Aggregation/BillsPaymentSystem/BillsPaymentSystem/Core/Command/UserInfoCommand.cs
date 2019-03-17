using BillsPaymentSystem.App.Core.Command.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Command
{
    public class UserInfoCommand : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public UserInfoCommand(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            StringBuilder sb = new StringBuilder();

            int userId = int.Parse(args[0]);

            var user = this.context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                throw new ArgumentNullException("User is not found!");
            }

            sb.AppendLine($"User: {user.FirstName} {user.LastName}");
            sb.AppendLine($"Bank Accounts:");

            var bankAccounts = user.PaymentMethods
                                   .Where(x => x.BankAccount != null)
                                   .Select(x => x.BankAccount)
                                   .ToArray();
                                                

            foreach (var bankAccount in bankAccounts)
            {
                sb.AppendLine($"-- ID: {bankAccount.BankAccountId}");
                sb.AppendLine($"--- Balance: {bankAccount.Balance:F2}");
                sb.AppendLine($"--- Bank: {bankAccount.BankName}");
                sb.AppendLine($"--- SWIFT: {bankAccount.SWIFT}");
            }

            sb.AppendLine($"Credit Cards:");

            var creditCards = user.PaymentMethods
                       .Where(x => x.CreditAccount != null)
                       .Select(x => x.CreditAccount)
                       .ToArray();

            foreach (var creditCard in creditCards)
            {
                sb.AppendLine($"-- ID: {creditCard.CreditCardId}");
                sb.AppendLine($"--- Limit: {creditCard.Limit:F2}");
                sb.AppendLine($"--- MoneyOwed: {creditCard.MoneyOwed:F2}");
                sb.AppendLine($"--- Limit Left: {creditCard.LimitLeft:F2}");
                sb.AppendLine($"--- Expiration Date: {creditCard.ExpirationDate.ToString("yyyy/MM")}");
            }

            return sb.ToString().Trim();
        }
    }
}

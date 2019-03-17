using BillsPaymentSystem.App.Core.Command.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillsPaymentSystem.App.Core.Command
{
    public class UserInfoComment : ICommand
    {
        private readonly BillsPaymentSystemContext context;

        public UserInfoComment(BillsPaymentSystemContext context)
        {
            this.context = context;
        }

        public string Execute(string[] args)
        {
            int userId = int.Parse(args[0]);

            var user = this.context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user == null)
            {
                throw new ArgumentNullException("User is not found!");
            }

            return "";
        }
    }
}

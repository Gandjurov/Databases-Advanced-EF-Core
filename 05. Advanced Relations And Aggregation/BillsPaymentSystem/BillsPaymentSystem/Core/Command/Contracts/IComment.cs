using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.App.Core.Command.Contracts
{
    public interface IUserInfoComment
    {
        string Execute(string[] args);
    }
}

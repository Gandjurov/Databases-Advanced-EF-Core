using AutoMapper;
using MyApp.Core.Commands.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ExitCommand : ICommand
    {
        private readonly MyAppContext context;
        private readonly Mapper mapper;

        public ExitCommand(MyAppContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            Environment.Exit(0);
            return string.Empty;
        }
    }
}

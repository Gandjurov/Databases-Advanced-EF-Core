using System;

namespace MiniORM
{
	// TODO: Create your DbContext class here.
    public class DbContext
    {
        public static Type[] AllowedSqlTypes =
        {
            typeof(bool),
            typeof(string),
            typeof(int),
            typeof(uint),
            typeof(DateTime),
            typeof(double),
            typeof(decimal),
            typeof(long),
            typeof(ulong)
        };
    }

}
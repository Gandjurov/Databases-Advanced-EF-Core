using System;
using System.Collections.Generic;
using System.Text;

namespace CustomAutoMapper
{
    public class ReflectionUtils
    {
        private static readonly HashSet<string> types = new HashSet<string> { "System.String", "System.Int32", "System.Decimal",
                "System.Double", "System.Guid", "System.Single", "System.Int64", "System.UInt64",
                "System.Int16", "System.DateTime", "System.String[]", "System.Int32[]", "System.Decimal[]", "System.Double[]", "System.Guid[]", "System.Single[]", "System.DateTime[]"
        };

        public static bool IsPrimitive(Type type)
        {
            return types.Contains(type.FullName)
                || type.IsPrimitive
                || IsNullable(type) && IsPrimitive(Nullable.GetUnderlyingType(type))
                || type.IsEnum;
        }

        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}

using System;
using System.Reflection;

namespace DoddleReport
{
    public static class ReflectionExtensions
    {
        public static T GetProperty<T>(this object item, string property)
        {
            var pi = item.GetType().GetProperty(property);
            if(pi != null)
            {
                return (T)pi.GetValue(item, null);
            }

            return default(T);
        }

        public static bool HasAttribute(this Type t, Type attrType)
        {
            return t.GetCustomAttributes(attrType, true) != null;
        }

        public static bool HasAttribute(this MemberInfo mi, Type attrType)
        {
            return mi.GetCustomAttributes(attrType, false) != null;
        }

        /// <summary> 
        /// Determines if a type is numeric.  Nullable numeric types are considered numeric. 
        /// </summary> 
        /// <remarks> 
        /// Boolean is not considered numeric. 
        /// </remarks> 
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }

                    return false;
            }

            return false;
        }
    }
}

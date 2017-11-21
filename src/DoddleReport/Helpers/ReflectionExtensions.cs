using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DoddleReport
{
    internal static class ReflectionExtensions
    {
        public static bool _IsEnum(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsClass;
#else
            return type.IsEnum;
#endif
        }

        public static bool _IsGenericType(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type itemType)
        {
#if NETSTANDARD1_5
            return itemType?.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance);
#else
            return itemType?.GetProperties(BindingFlags.Public | BindingFlags.Instance);
#endif
        }

        public static T GetProperty<T>(this object item, string property)
        {
#if NETSTANDARD1_5
            var pi = item.GetType().GetTypeInfo().GetProperty(property);
            if (pi != null)
            {
                return (T)pi.GetValue(item, null);
            }
#else
            var pi = item.GetType().GetProperty(property);
            if (pi != null)
            {
                return (T)pi.GetValue(item, null);
            }
#endif

            return default(T);
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member)
            where TAttribute : Attribute
        {
#if NETSTANDARD1_5
            return member.GetCustomAttributes<TAttribute>(inherit: false).FirstOrDefault();
#else
            return member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().FirstOrDefault();
#endif
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
#if NETSTANDARD1_5
                    if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
#else
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
#endif

                    return false;
            }

            return false;
        }

        public static Type GetGenericDataType(this Type type)
        {
#if NETSTANDARD1_5
            return type.GetTypeInfo().IsGenericType ? type.GetTypeInfo().GetGenericArguments().Single() : type; ;
#else
            return type.IsGenericType ? type.GetGenericArguments().Single() : type;
#endif
        }
    }


}

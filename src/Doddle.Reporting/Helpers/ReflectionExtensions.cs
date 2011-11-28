using System;
using System.Reflection;

namespace Doddle.Reporting
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
    }
}

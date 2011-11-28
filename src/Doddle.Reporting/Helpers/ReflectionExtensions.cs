using System;
using System.Web.UI;
using System.Reflection;

namespace Doddle.Reporting
{
    public static class ReflectionExtensions
    {
        // TODO: Remove dependency on System.Web/DataBinder
        public static T GetProperty<T>(this object item, string expresssion)
        {
            return (T)DataBinder.Eval(item, expresssion);
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

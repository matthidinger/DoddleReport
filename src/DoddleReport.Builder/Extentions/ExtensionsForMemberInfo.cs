using System;
using System.Linq;
using System.Reflection;

namespace DoddleReport.Builder.Extentions
{
    public static class ExtensionsForMemberInfo
    {
        public static bool HasAttribute<TAttribute>(this MemberInfo @this)
        {
            return HasAttribute(@this, typeof(TAttribute));
        }

        public static bool HasAttribute(this MemberInfo @this, Type attributeType)
        {
            return Attribute.GetCustomAttributes(@this, attributeType).Any();
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo @this) where TAttribute : Attribute
        {
            if (!@this.HasAttribute(typeof(TAttribute)))
                throw new InvalidOperationException($"Member {@this} has no attribute of type {typeof(TAttribute)}");
            return (TAttribute)@this.GetCustomAttribute(typeof(TAttribute));
        }


    }
}

using System.ComponentModel;
using System.Reflection;

namespace Zaz.Server.Utils
{
    static class TypeExtensions
    {
        public static TAttr FindAttribute<TAttr>(this ICustomAttributeProvider @this)
            where TAttr : class
        {
            var attributes = @this.GetCustomAttributes(typeof(TAttr), true);
            if (attributes.Length > 0)
            {
                return (TAttr)attributes[0];
            }

            return null;
        }

        public static string GetDescriptionOrNull(this ICustomAttributeProvider @this)
        {
            var descriptionAttribute = @this.FindAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return null;
        }
    }
}

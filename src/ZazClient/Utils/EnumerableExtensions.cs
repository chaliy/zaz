using System.Collections.Generic;

namespace Zaz.Client
{
    static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> @this)            
        {
            if (@this == null)
            {
                return new T[0];
            }
            return @this;
        }        
    }
}

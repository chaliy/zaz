using System;
using System.Linq;

namespace Zaz.Server
{
    public static class DefaultConventions
    {
        public static readonly Func<string, Type> CommandResolver =
            key => AppDomain.CurrentDomain
                       .GetAssemblies()
                       .Select(a => a.GetType(key, false, true))
                       .Where(x => x != null)
                       .FirstOrDefault();
        
        public static readonly ICommandStateProvider CommandStateProvider =
        	new MemoryCommandStateProvider();
    }
}

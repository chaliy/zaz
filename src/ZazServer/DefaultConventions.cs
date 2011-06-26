using System;
using System.Reflection;

namespace Zaz.Server
{
    public static class DefaultConventions
    {
        public static readonly Func<string, Type> CommandResolver =
            key => Assembly.GetEntryAssembly().GetType(key, false, true);
    }
}

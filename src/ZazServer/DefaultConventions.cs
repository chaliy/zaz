using System;
using System.Reflection;

namespace Zaz.Server
{
    public static class DefaultConventions
    {
        public static readonly ICommandStateProvider CommandStateProvider =
        	new MemoryCommandStateProvider();

        public static readonly ICommandBroker CommandBroker =
            new ReflectionCommandBroker(Assembly.GetEntryAssembly());

        public static readonly ICommandRegistry CommandRegistry =
            new ReflectionCommandRegistry(AppDomain.CurrentDomain
                                            .GetAssemblies());
    }
}

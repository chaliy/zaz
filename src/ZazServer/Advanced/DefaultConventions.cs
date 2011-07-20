using System;
using System.Reflection;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced
{
    public static class DefaultConventions
    {
        public static readonly ICommandStateProvider StateProvider =
        	new MemoryCommandStateProvider();

        public static readonly ICommandBroker Broker =
            new ReflectionCommandBroker(Assembly.GetEntryAssembly());

        public static readonly ICommandRegistry CommandRegistry =
            new ReflectionCommandRegistry(AppDomain.CurrentDomain
                                            .GetAssemblies());
    }
}

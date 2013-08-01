using System;
using System.Reflection;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced
{
    public static class Implementations
    {
        public static readonly Lazy<ICommandStateProvider> StateProvider = new Lazy<ICommandStateProvider>(() => new MemoryCommandStateProvider());
        public static readonly Lazy<ICommandBroker> Broker = new Lazy<ICommandBroker>(() => new ReflectionCommandBroker(Assembly.GetEntryAssembly()));
        public static readonly Lazy<ICommandRegistry> CommandRegistry = new Lazy<ICommandRegistry>(() => new ReflectionCommandRegistry(AppDomain.CurrentDomain.GetAssemblies()));
    }
}

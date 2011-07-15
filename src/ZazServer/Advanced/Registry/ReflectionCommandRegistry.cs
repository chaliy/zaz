using System;
using System.Linq;
using System.Reflection;

namespace Zaz.Server.Advanced.Registry
{
    public class ReflectionCommandRegistry : ICommandRegistry
    {
        private readonly Assembly[] _commandsAssemblies;
        private readonly Func<Type, bool> _filter;

        public ReflectionCommandRegistry(params Assembly[] commandsAssemblies)
            : this(commandsAssemblies, null)
        {
        }

        public ReflectionCommandRegistry(Assembly[] commandsAssemblies, Func<Type, bool> filter)
        {
            _commandsAssemblies = commandsAssemblies;
            _filter = filter ?? (t => true);
        }

        public IQueryable<CommandInfo> Query()
        {
            return _commandsAssemblies
                .SelectMany(x => x.GetTypes())                
                .Where(x => x.IsClass
                    && !x.IsGenericType
                    && !x.IsAutoClass
                    && x.GetConstructors().Any(xx => xx.GetParameters().Length == 0))
                .Where(_filter)
                .Select(CommandInfoFactory.Create)
                .AsQueryable();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zaz.Server.Advanced.Registry
{
    public class ReflectionCommandRegistry : ICommandRegistry
    {
        private readonly Assembly[] _commandsAssemblies;
        private readonly List<Type> _types = new List<Type>();

        public ReflectionCommandRegistry()
        {
        }

        public ReflectionCommandRegistry(params Assembly[] commandsAssemblies)            
        {
            _commandsAssemblies = commandsAssemblies;                        
        }

        public ReflectionCommandRegistry(params Type[] commandTypes)
        {
            _types.AddRange(commandTypes ?? new Type[0]);
        }
                
        public Func<Type, bool> Filter { get; set; }
        public Func<Type, CommandInfo> InfoFactory { get; set; }
        
        public IQueryable<CommandInfo> Query()
        {
            return (_commandsAssemblies ?? new Assembly[0])
                .SelectMany(x => x.GetTypes())
                .Union(_types)
                .Where(x => x.IsClass
                    && !x.IsGenericType
                    && !x.IsAutoClass
                    && x.GetConstructors().Any(xx => xx.GetParameters().Length == 0))
                .Where(Filter ?? (t => true))
                .Select(InfoFactory ?? CommandInfoFactory.Create)
                .AsQueryable();
        }
    }
}
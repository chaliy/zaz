using System;
using System.Collections.Generic;

namespace SampleCommands
{
    public static class CommandRegistry
    {
        public static IEnumerable<Type> ResolveCommand(string key)
        {
            var commandType = Type.GetType(key + ", SampleCommands", false);
            if (commandType != null)
            {
                yield return commandType;
            }        
        }        
    }
}

using System;

namespace SampleCommands
{
    public static class CommandRegistry
    {
        public static Type GetCommand(string key)
        {
            return Type.GetType("SampleCommands." + key + ", SampleCommands");
        }
    }
}

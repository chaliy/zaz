using System;

namespace SampleCommands
{
    public static class CommandRegistry
    {
        public static Type GetCommand(string key)
        {
            return Type.GetType("Inventory.Domain.Commands." + key + ", Inventory.Domain");
        }
    }
}

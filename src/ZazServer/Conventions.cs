using System;

namespace Zaz.Server
{
    public class Conventions
    {
        public Func<string, Type> CommandResolver { get; set; }
    }
}

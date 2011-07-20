using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zaz.Server.Advanced.Broker;

namespace Zaz.Tests.Server.Stubs
{
    public class CommandBrokerStub : ICommandBroker
    {
        public readonly List<object> HandledCommands = new List<object>();

        public Task<dynamic> Handle(object cmd, CommandHandlingContext ctx)
        {
            HandledCommands.Add(cmd);
            return Task.Factory.StartNew(() =>
                                             {
                                                 Console.WriteLine("Do nothing!");
                                                 return default(object);
                                             });
        }        
    }
}

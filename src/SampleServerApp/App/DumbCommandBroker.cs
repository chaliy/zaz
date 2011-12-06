using System;
using System.Threading;
using System.Threading.Tasks;
using SampleCommands;
using Zaz.Server.Advanced.Broker;

namespace SampleServerApp.App
{
    public class DumbCommandBroker : ICommandBroker
    {        
        public Task Handle(object cmd, CommandHandlingContext ctx)
        {
            if (cmd.GetType() == typeof(LongFoo1))
            {
                return Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(300);
                    ctx.Log.Info("Hello word! #1");
                    Thread.Sleep(1000);
                    ctx.Log.Info("Hello word! #2");
                    Thread.Sleep(1000);
                    ctx.Log.Error("Hello word! #3");
                    Thread.Sleep(1000);
                    ctx.Log.Info("Hello word! #4");
                });
            }

            throw new InvalidOperationException("Dumb command handler cannot find how to handle " + cmd);
        }
    }
}
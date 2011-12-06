using System.Threading;
using System.Threading.Tasks;
using Zaz.Server.Advanced.Broker;

namespace Zaz.Tests.Integration.CustomBroker
{
    public class LongCommandBroker : ICommandBroker
    {
        public Task Handle(dynamic cmd, CommandHandlingContext ctx)
        {
            return Task.Factory.StartNew(() =>
                                      {
                                          ctx.Log.Info("Hello word! #1");
                                          Thread.Sleep(1000);
                                          ctx.Log.Info("Hello word! #2");
                                          Thread.Sleep(1000);
                                          ctx.Log.Info("Hello word! #3");
                                          Thread.Sleep(1000);
                                          ctx.Log.Info("Hello word! #4");
                                      });
        }
    }
}

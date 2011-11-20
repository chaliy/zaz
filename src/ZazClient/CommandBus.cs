using System;
using System.Net.Http;
using System.Threading.Tasks;
using Zaz.Client.Avanced;

namespace Zaz.Client
{
    public class CommandBus
    {
        private readonly AdvancedCommandBus _underlineBus;

        public CommandBus(string url, ZazConfiguration configuration = null)
        {
            _underlineBus = new AdvancedCommandBus(url, configuration);
        }

        public void Post(object cmd, params string[] tags)
        {
            _underlineBus.Post(new CommandEnvelope
                                   {
                                       Key = cmd.GetType().FullName,
                                       Command = cmd,
                                       Tags = tags
                                   }).Wait();
        }

        public Task PostAsync(object cmd, params string[] tags)
        {
            return _underlineBus.Post(new CommandEnvelope
            {
                Key = cmd.GetType().FullName,
                Command = cmd,
                Tags = tags
            });
        }
    }
}

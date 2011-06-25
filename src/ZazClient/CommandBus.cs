using Zaz.Client.Avanced;

namespace Zaz.Client
{
    public class CommandBus
    {
        private readonly AdvancedCommandBus _underlineBus;

        public CommandBus(string url)
        {
            _underlineBus = new AdvancedCommandBus(url);
        }

        public void Post(object cmd)
        {
            _underlineBus.Post(new CommandEnvelope
                                   {
                                       Key = cmd.GetType().FullName,
                                       Command = cmd
                                   }).Wait();
        }
    }
}

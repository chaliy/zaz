using System.Threading.Tasks;

namespace Zaz.Server.Advanced.Broker
{
    public interface ICommandBroker
    {
        Task Handle(object cmd, CommandHandlingContext ctx);
    }
}
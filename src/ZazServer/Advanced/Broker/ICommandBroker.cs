using System.Threading.Tasks;

namespace Zaz.Server.Advanced.Broker
{
    public interface ICommandBroker
    {
        Task<object> Handle(object cmd, CommandHandlingContext ctx);
    }
}
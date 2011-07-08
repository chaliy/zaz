using System.Threading.Tasks;

namespace Zaz.Server
{
    public interface ICommandBroker
    {
        Task Handle(object cmd, CommandHandlingContext ctx);
    }
}
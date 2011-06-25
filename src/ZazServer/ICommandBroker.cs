using System;
using System.Threading.Tasks;

namespace Zaz.Server
{
    public interface ICommandBroker
    {
        Type ResolveCommandType(string key);

        Task Handle(object cmd);
    }
}
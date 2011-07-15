using System.Linq;

namespace Zaz.Server.Advanced.Registry
{
    public interface ICommandRegistry
    {
        IQueryable<CommandMeta> Query();
    }
}

using System.Linq;

namespace Zaz.Server
{
    public interface ICommandRegistry
    {
        IQueryable<CommandMeta> Query();
    }
}

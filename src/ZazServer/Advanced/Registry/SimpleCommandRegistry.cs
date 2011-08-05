using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zaz.Server.Advanced.Registry
{
    public class SimpleCommandRegistry : ICommandRegistry, IEnumerable<CommandInfo>
    {        
        private readonly List<CommandInfo> _infos = new List<CommandInfo>();
                        
        public IQueryable<CommandInfo> Query()
        {
            return _infos
                .AsQueryable();
        }

        public void Add(CommandInfo info)
        {
            _infos.Add(info);
        }

        public IEnumerator<CommandInfo> GetEnumerator()
        {
            return _infos.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public class MemoryCommandStateProvider : BaseCommandStateProvider
    {		
        private readonly IDictionary<string, List<LogEntry>> _storage = new Dictionary<string, List<LogEntry>>();
		
        public override IQueryable<LogEntry> QueryEntries(string key)
        {				
            if (_storage.ContainsKey(key))
            {
                return _storage[key].AsQueryable();
            }
            return Enumerable.Empty<LogEntry>().AsQueryable();
        }
		
        protected override void WriteEntry(string key, LogEntry entry)
        {
            if (!_storage.ContainsKey(key))
            {
                _storage[key] = new List<LogEntry>();
            }
			
            _storage[key].Add(entry);
        }		
    }
}
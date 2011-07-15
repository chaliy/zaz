using System.Collections.Generic;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public class MemoryCommandStateProvider : BaseCommandStateProvider
    {		
        private readonly IDictionary<string, List<TraceEntry>> _storage = new Dictionary<string, List<TraceEntry>>();
		
        public override IQueryable<TraceEntry> QueryEntries(string key)
        {				
            if (_storage.ContainsKey(key))
            {
                return _storage[key].AsQueryable();
            }
            return Enumerable.Empty<TraceEntry>().AsQueryable();
        }
		
        protected override void WriteEntry(string key, TraceEntry entry)
        {
            if (!_storage.ContainsKey(key))
            {
                _storage[key] = new List<TraceEntry>();
            }
			
            _storage[key].Add(entry);
        }		
    }
}
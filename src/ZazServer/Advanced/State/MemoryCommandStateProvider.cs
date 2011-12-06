using System.Collections.Generic;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public class MemoryCommandStateProvider : BaseCommandStateProvider
    {		
        private readonly IDictionary<string, List<ProgressEntry>> _storage = new Dictionary<string, List<ProgressEntry>>();
		
        public override IQueryable<ProgressEntry> QueryEntries(string key)
        {				
            if (_storage.ContainsKey(key))
            {
                return _storage[key].AsQueryable();
            }
            return Enumerable.Empty<ProgressEntry>().AsQueryable();
        }
		
        protected override void WriteEntry(string key, ProgressEntry entry)
        {
            if (!_storage.ContainsKey(key))
            {
                _storage[key] = new List<ProgressEntry>();
            }
			
            _storage[key].Add(entry);
        }		
    }
}
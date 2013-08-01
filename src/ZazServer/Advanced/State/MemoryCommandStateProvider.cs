using System.Collections.Concurrent;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public class MemoryCommandStateProvider : BaseCommandStateProvider
    {
        readonly ConcurrentDictionary<string, ConcurrentBag<ProgressEntry>> _storage = new ConcurrentDictionary<string, ConcurrentBag<ProgressEntry>>();

        public override IQueryable<ProgressEntry> QueryEntries(string key)
        {
            var res = _storage.GetOrAdd(key, new ConcurrentBag<ProgressEntry>());

            return res.ToArray().AsQueryable();
        }

        protected override void WriteEntry(string key, ProgressEntry entry)
        {
            _storage.AddOrUpdate(key, new ConcurrentBag<ProgressEntry>(new[] { entry }), (s, bag) =>
            {
                bag.Add(entry);
                return bag;
            });
        }
    }
}
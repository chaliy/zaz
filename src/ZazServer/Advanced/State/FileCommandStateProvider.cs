using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Zaz.Server.Advanced.State
{
    public class FileCommandStateProvider : BaseCommandStateProvider
    {
        private readonly DirectoryInfo _storage;
		
        public FileCommandStateProvider(String storagePath)
        {
            _storage = new DirectoryInfo(storagePath);
            if (!_storage.Exists)
            {
                _storage.Create();
            }
        }
						
        public override IQueryable<ProgressEntry> QueryEntries(string key)
        {
            var data = File.ReadAllText(GetCommandStatePath(key));
            return JsonConvert.DeserializeObject<IEnumerable<ProgressEntry>>("[" + data + "]").AsQueryable();
        }
		
        protected override void WriteEntry(string key, ProgressEntry entry)
        {
            var path = GetCommandStatePath(key);
            File.AppendAllText(path, JsonConvert.SerializeObject(entry) + ",");
        }
		
        private string GetCommandStatePath(string key)
        {
            return Path.Combine(_storage.FullName, key);
        }
    }
}
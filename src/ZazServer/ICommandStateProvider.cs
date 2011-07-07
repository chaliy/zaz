using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace Zaz.Server
{	
	public interface ICommandStateProvider
	{
		void Start(string key, DateTime timestamp);
		void WriteTrace(string key, DateTime timestamp, TraceSeverity severity, string message);
		void CompleteSuccess(string key, DateTime timestamp);
		void CompleteFailure(string key, DateTime timestamp, string reason);
		IQueryable<TraceEntry> QueryEntries(string key);
	}
	
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
						
		public override IQueryable<TraceEntry> QueryEntries(string key)
		{
			var data = File.ReadAllText(GetCommandStatePath(key));
			return JsonConvert.DeserializeObject<IEnumerable<TraceEntry>>("[" + data + "]").AsQueryable();
		}
		
		protected override void WriteEntry(string key, TraceEntry entry)
		{
			var path = GetCommandStatePath(key);
			File.AppendAllText(path, JsonConvert.SerializeObject(entry) + ",");
		}
		
		private string GetCommandStatePath(string key)
		{
			return Path.Combine(_storage.FullName, key);
		}
	}
	
	public abstract class BaseCommandStateProvider : ICommandStateProvider
	{				
		public void Start(string key, DateTime timestamp)
		{			
			WriteEntry(key, new TraceEntry
			           {
			           		Kind = TraceKind.Start,
			           		Timestamp = timestamp
			           });
		}
		
		public void WriteTrace(string key, DateTime timestamp, TraceSeverity severity, string message)
		{
			WriteEntry(key, new TraceEntry
			           {
			           		Kind = TraceKind.Trace,
			           		Timestamp = timestamp,
							Severity = severity,
							Message = message
			           });
		}
		
		public void CompleteSuccess(string key, DateTime timestamp)
		{
			WriteEntry(key, new TraceEntry
			           {
			           		Kind = TraceKind.Success,
			           		Timestamp = timestamp													
			           });
		}
		
		public void CompleteFailure(string key, DateTime timestamp, string reason)
		{
			WriteEntry(key, new TraceEntry
			           {
			           		Kind = TraceKind.Failure,
			           		Timestamp = timestamp													
			           });
		}
		
		public abstract IQueryable<TraceEntry> QueryEntries(string key);
		
		protected abstract void WriteEntry(string key, TraceEntry entry);
	}
	
	public class TraceEntry 
	{
		public TraceKind Kind {get;set;}
		public DateTime Timestamp {get;set;}
		public TraceSeverity Severity {get;set;}
		public string Message {get;set;}
	}
	
	public enum TraceKind 
	{
		Start,
		Trace,
		Success,
		Failure
	}
	
	public enum TraceSeverity 
	{
		Info,
		Warning,
		Error
	}
}

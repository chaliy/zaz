using System;
using System.Linq;

namespace Zaz.Server.Advanced.State
{	
	public interface ICommandStateProvider
	{
		void Start(string key, DateTime timestamp);
		void WriteTrace(string key, DateTime timestamp, LogEntrySeverity severity, string message, string[] tags);
		void CompleteSuccess(string key, DateTime timestamp);
		void CompleteFailure(string key, DateTime timestamp, string reason);
		IQueryable<LogEntry> QueryEntries(string key);
	}
}

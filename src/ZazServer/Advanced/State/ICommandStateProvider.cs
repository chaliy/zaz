using System;
using System.Linq;

namespace Zaz.Server.Advanced.State
{	
	public interface ICommandStateProvider
	{
		void Start(string key, DateTime timestamp);
		void WriteTrace(string key, DateTime timestamp, TraceSeverity severity, string message);
		void CompleteSuccess(string key, DateTime timestamp);
		void CompleteFailure(string key, DateTime timestamp, string reason);
		IQueryable<TraceEntry> QueryEntries(string key);
	}
}

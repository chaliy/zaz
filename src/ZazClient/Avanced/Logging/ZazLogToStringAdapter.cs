using System;
using System.Text;

namespace Zaz.Client.Avanced.Logging
{
    public class ZazLogToStringAdapter : IObserver<LogEntry>
    {            
        readonly StringBuilder _buffer = new StringBuilder();

        public bool IncludeTraces { get; set; }

        public void OnNext(LogEntry entry)
        {
            if (entry.Severity == LogEntrySeverity.Trace && !IncludeTraces)
            {
                return;
            }

            _buffer.AppendLine("[" + entry.Severity + "]" + entry.Message);            
                
        }

        public void OnError(Exception error)
        {                
        }

        public void OnCompleted()
        {
        }

        public bool HasSomething()
        {
            return _buffer.Length > 0;
        }

        public override string ToString()
        {            
            return "Log:\r\n" + _buffer;   
        }
    }
}
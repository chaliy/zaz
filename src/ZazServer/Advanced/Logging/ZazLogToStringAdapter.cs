using System;
using System.Text;

namespace Zaz.Server.Advanced.Logging
{
    public class ZazLogToStringAdapter : IObserver<LogEntry>
    {            
        readonly StringBuilder _buffer = new StringBuilder();

        public bool IncludeTraces { get; set; }

        public void OnNext(LogEntry entry)
        {
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
            return _buffer.ToString();   
        }
    }
}
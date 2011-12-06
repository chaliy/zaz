using System;
using System.Diagnostics;

namespace Zaz.Client.Avanced.Logging
{
    public class ZazLogToSystemTraceAdapter : IObserver<LogEntry>
    {
        public bool WriteTraces { get; set; }

        public void OnNext(LogEntry value)
        {
            switch (value.Severity)
            {
                case LogEntrySeverity.Trace:
                    if (WriteTraces)
                    {
                        Trace.TraceInformation(">>" + value.Message);
                    }
                    break;
                case LogEntrySeverity.Info:
                    Trace.TraceInformation(value.Message);
                    break;
                case LogEntrySeverity.Warning:
                    Trace.TraceWarning(value.Message);
                    break;
                case LogEntrySeverity.Error:
                    Trace.TraceError(value.Message);
                    break;                    
            }
                
        }

        public void OnError(Exception error)
        {                
        }

        public void OnCompleted()
        {
        }
    }
}
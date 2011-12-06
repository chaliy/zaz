using System.Collections.Generic;

namespace Zaz.Server.Advanced.Executor
{
    public class ExecutionStats
    {
        public ExecutionId Id { get; set; }
        public ExecutionStatus Status { get; set; }
        public List<LogEntry> Log { get; set; }
    }
}

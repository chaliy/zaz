namespace Zaz.Server.Advanced.Service.Contract
{
    public class GetScheduledCommandResponse
    {
        public string Id { get; set; }
        public ScheduledCommandStatus Status { get; set; }

        public TraceEntry[] Trace { get; set; }
    }
}

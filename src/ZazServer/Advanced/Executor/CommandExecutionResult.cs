namespace Zaz.Server.Advanced.Executor
{
    public class CommandExecutionResult
    {
        public bool IsSucces { get; private set; }
        public string FailureReason { get; private set; }

        public static CommandExecutionResult Success()
        {
            return new CommandExecutionResult { IsSucces = true };
        }

        public static CommandExecutionResult Failure(string reason = "N/A")
        {
            return new CommandExecutionResult { IsSucces = false, FailureReason = reason };
        }
    }
}

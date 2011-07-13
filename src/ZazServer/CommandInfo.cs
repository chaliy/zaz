namespace Zaz.Server
{
    public class CommandInfo
    {
        public string Key { get; set; }
        public string[] Aliases { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public CommandParameterInfo[] Parameters { get; set; }
    }
}

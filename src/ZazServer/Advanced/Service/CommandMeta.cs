namespace Zaz.Server.Advanced.Service
{
    public class CommandMeta
    {
        public string Key { get; set; }
        public string[] Aliases { get; set; }
        public string Description { get; set; }
        public string[] Tags { get; set; }
        public CommandMetaParameter[] Parameters { get; set; }
    }
}

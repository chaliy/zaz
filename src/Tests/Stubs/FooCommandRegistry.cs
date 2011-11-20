using Zaz.Server.Advanced.Registry;
namespace Zaz.Tests.Stubs
{
    public class FooCommandRegistry : SimpleCommandRegistry
    {
        public FooCommandRegistry()
        {
            Add(CommandInfoFactory.Create(typeof(FooCommand)));
        }
    }

    public class FooCommand
    {
        public string Message { get; set; }
    }
}

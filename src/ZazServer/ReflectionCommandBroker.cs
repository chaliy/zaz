using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Zaz.Server
{
    public class ReflectionCommandBroker : ICommandBroker
    {
        private readonly Assembly _handlersAssembly;

        public ReflectionCommandBroker(Assembly handlersAssembly)
        {
            _handlersAssembly = handlersAssembly;
        }

        public Task Handle(dynamic cmd, CommandHandlingContext ctx)
        {
            var cmdType = cmd.GetType();
            var handlerType = _handlersAssembly
                .GetTypes()
                .Where(x => x.Name.EndsWith("Handler")
                            && x.GetMethod("Handle") != null
                            && x.GetMethod("Handle")
                                   .GetParameters()[0].ParameterType
                                   .IsAssignableFrom(cmdType))
                .FirstOrDefault();

            if (handlerType == null)
            {
                throw new InvalidOperationException("Zaz failed to find appropriate handler");
            }

            dynamic handler = Activator.CreateInstance(handlerType);

            return Task.Factory.StartNew(() =>
            {
                handler.Handle(cmd);
            });
        }
    }
}

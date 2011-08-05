using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Zaz.Server.Advanced.Broker
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

            var result = handler.Handle(cmd);

            if (result is Task)
            {
                return (Task) result;
            }

            return Task.Factory.StartNew(() => {});
        }
    }
}

using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zaz.Server.Advanced.Service.Contract;

namespace Zaz.Server.Advanced.Service
{
    public class CommandResolver
    {
        private readonly Conventions _conventions;

        public CommandResolver(Conventions conventions)
        {
            _conventions = conventions;
        }

        public object ResoveCommand(PostScheduledCommandRequest env, string cmdKey)
        {
            if (String.IsNullOrWhiteSpace(cmdKey))
            {
                throw ExceptionsFactory.CreateApiException("Required value 'Key' was not found.");
            }

            var cmdType = ResolveCommandType(cmdKey);
            var cmd = BuildCommand(env, cmdType);
            return cmd;
        }

        public Type ResolveCommandType(string key)
        {
            var cmdType = (_conventions.CommandRegistry
                           ?? DefaultConventions.CommandRegistry)
                .Query()
                .Where(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Type)
                .FirstOrDefault();

            if (cmdType == null)
            {
                throw ExceptionsFactory.CreateApiException("Command " + key + " was not found");
            }

            return cmdType;
        }

        private static object BuildCommand(dynamic env, Type cmdType)
        {
            try
            {
                if (env.Command != null)
                {
                    var serializer = new JsonSerializer();
                    var reader = ((JObject)env.Command).CreateReader();
                    var cmd = serializer.Deserialize(reader, cmdType);
                    return cmd;
                }
            }
            catch (JsonReaderException ex)
            {
                throw ExceptionsFactory.CreateApiException("Problems with deserializing command data. " + ex.Message);
            }

            return Activator.CreateInstance(cmdType);
        }

        
    }
}

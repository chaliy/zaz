using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zaz.Server.Advanced.Registry;
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
            var query = (_conventions.Registry ?? DefaultConventions.CommandRegistry).Query();

            var matches = query
                .Where(x => x.Key.Contains(key))
                .Union(query.Where(x => (x.Aliases ?? new string[0]).Any(xx => xx.Contains(key))))
                .Distinct()
                .ToList();
            
            if (matches.Count == 1)
            {                
                return matches[0].Type;    
            }

            if (matches.Count > 1)
            {
                throw ExceptionsFactory.CreateApiException("More then one match for command " + key + " was found.");                
            }

            throw ExceptionsFactory.CreateApiException("Command " + key + " was not found.");
                                
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

using System;
using System.Linq;
using System.Linq.Expressions;
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

        private CommandInfo TryResolveSingle(Expression<Func<CommandInfo, bool>> predicate)
        {
            var query = (_conventions.Registry ?? DefaultConventions.CommandRegistry).Query();

            var matches = query
                .Where(predicate)
                .ToList();

            if (matches.Count == 1)
            {
                return matches[0];
            }

            return null;
        }
        
        public Type ResolveCommandType(string key)
        {
            // Try exact match
            var cmd = TryResolveSingle(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            
            if (cmd == null)
            {
                // Try exact match on alias
                cmd = TryResolveSingle(x => (x.Aliases ?? new string[0]).Any(xx => xx.Equals(key, StringComparison.OrdinalIgnoreCase)));
            }

            if (cmd == null)
            {
                // Try contains on key
                cmd = TryResolveSingle(x => Contains(x.Key, key));
            }

            if (cmd == null)
            {
                // Try contains on alias
                cmd = TryResolveSingle(x => (x.Aliases ?? new string[0]).Any(xx => Contains(key, xx)));
            }

            if (cmd != null)
            {
                return cmd.Type;
            }

            throw ExceptionsFactory.CreateApiException("Command " + key + " was not found.");
        }

        private static bool Contains(string s1, string s2)
        {            
            return (s1 ?? "").ToLower().Contains((s2 ?? "").ToLower());
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

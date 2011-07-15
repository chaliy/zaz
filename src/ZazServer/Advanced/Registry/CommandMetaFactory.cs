using System;
using System.Linq;
using System.Reflection;
using Zaz.Server.Utils;

namespace Zaz.Server.Advanced.Registry
{
    public static class CommandMetaFactory
    {
        public static CommandMeta Create(Type commandType)
        {
            var key = commandType.FullName;            
            var description = commandType.GetDescriptionOrNull();
            var parameters = commandType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => new CommandParameterInfo
                                 {
                                     Name = x.Name,
                                     Type = Type.GetTypeCode(x.PropertyType).ToString(),
                                     Description = x.GetDescriptionOrNull()
                                 })
                .ToArray();

            return new CommandMeta
                       {
                           Type = commandType,
                           Info = new CommandInfo
                                      {
                                          Key = key,
                                          Description = description,
                                          Parameters = parameters
                                      }
                       };
        }        
    }
}

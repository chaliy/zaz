using System;
using System.Collections.Generic;

namespace Zaz.Server
{
    public class CommandBinder
    {
        public object Build(Type cmdType, IDictionary<string, string> data)
        {
            var cmd = Activator.CreateInstance(cmdType);

            foreach (var key in data.Keys)
            {
                var prop = cmdType.GetProperty(key);
                if (prop != null)
                {
                    prop.SetValue(cmd, data[key], null);
                }
            }

            return cmd;
        }
    }
}

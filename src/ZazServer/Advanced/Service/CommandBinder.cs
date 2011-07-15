using System;
using System.Collections.Generic;

namespace Zaz.Server.Advanced.Service
{
    public class CommandBinder
    {
        public object Build(Type cmdType, IDictionary<string, string> data)
        {
            var cmd = Activator.CreateInstance(cmdType);            

            foreach(var prop in cmdType.GetProperties())
            {                
                if (!data.ContainsKey(prop.Name))
                {
                    throw new InvalidOperationException("Value for required property " + prop.Name + " was not found");
                }
                var val = NormalizeValue(prop.PropertyType, data[prop.Name]);
                prop.SetValue(cmd, val, null);
            }            
           
            return cmd;
        }

        private static object NormalizeValue(Type t, string v){
            if (t == typeof(Guid))
            {
                return new Guid(v);                
            }
            else if (t.IsEnum)
            {
                return Enum.Parse(t, v);
            }
            else if (t == typeof(Boolean))
            {
                return Boolean.Parse(v);
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                return NormalizeValue(Nullable.GetUnderlyingType(t), v);
            else
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(t);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFromString(v);
                }
            }
            return v;
        }
    }
}

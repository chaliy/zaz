using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Zaz.Server.Advanced.Executor
{
    [TypeConverter(typeof(ExecutionIdTypeConverter))]
    public struct ExecutionId
    {
        readonly string _value;

        private ExecutionId(string value)
        {
            _value = value;
        }

        [DebuggerStepThrough]
        public static implicit operator string(ExecutionId value)
        {
            return value._value;
        }

        public override string ToString()
        {
            return _value;
        }

        public static ExecutionId New()
        {
            return new ExecutionId(Guid.NewGuid().ToString("n"));
        }

        public static ExecutionId FromString(string id)
        {
            return new ExecutionId(id);
        }
    }
}

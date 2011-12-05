using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zaz.Client
{
    [Serializable]
    public class ZazException : Exception
    {
        public ZazException() { }

        public ZazException( string message ) : base( message ) { }

        public ZazException( string message, Exception inner ) : base( message, inner ) { }

        protected ZazException( 
        System.Runtime.Serialization.SerializationInfo info, 
        System.Runtime.Serialization.StreamingContext context ) : base( info, context ) 
        {
        }

        public static ZazException CreateDefault(Exception inner)
        {            
            return new ZazException("ZAZ failed to post command. " + inner.Message, inner);
        }
    }    
}

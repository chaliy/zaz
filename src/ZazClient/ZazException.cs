using System;

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
            return new ZazException("ZAZ failed to post command.", inner);
        }

        public override string ToString()
        {     
            // Standard implementation of the ToString calls ToString(bool) of the InnerException
            // this means that overrided ToString() of the InnerExpcetion is ignored
            // On the other hand, currect hack (reimplemened ToString()), fixes this issue            
            var text = GetType() + ":" + Message;            
            if (InnerException != null)
            {
                text += " ---> " + InnerException + "\r\n   --- End of inner exception stack trace ---";
            }            
            var stackTrace = StackTrace;
            if (stackTrace != null)
            {
                text = text + Environment.NewLine + stackTrace;
            }
            return text;
        }
    }    
}

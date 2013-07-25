using System;
using System.Net.Http;

namespace Zaz.Client.Avanced
{        
    public class ZazTransportException : System.Exception
    {
        public ZazTransportException() { }

        public ZazTransportException(string message) : base(message) { }

        public ZazTransportException(string message, HttpResponseMessage resp) : base(message) 
        {
            Response = resp;
        }

        public ZazTransportException(string message, System.Exception inner) : base(message, inner) { }

        public ZazTransportException(string message, HttpResponseMessage resp,  System.Exception inner)
            : base(message, inner)
        {
            Response = resp;
        }

        public HttpResponseMessage Response { get; private set; }

        public override string ToString()
        {
            return Message 
                + " Server response: \r\n" + Response 
                + "\r\n\r\nContent: \r\n" + Response.Content.ReadAsStringAsync().Result 
                + "\r\n\r\nStack trace:\r\n" + StackTrace; 
        }
    }
}

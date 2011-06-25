using System.Net.Http;
using Newtonsoft.Json;

namespace Zaz.Client.Avanced
{
    public class CommandContent : StringContent
    {
        public CommandContent(CommandEnvelope envelope) 
            : base(Serialize(envelope))
        {
        }

        public static string Serialize(object envelope)
        {            
            return JsonConvert.SerializeObject(envelope);
        }        
    }
}

using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Zaz.Client.Avanced
{
    public class CommandContent : StringContent
    {
        public CommandContent(CommandEnvelope envelope) 
            : base(Serialize(envelope), Encoding.UTF8, "application/json")
        {            
        }

        public static string Serialize(object envelope)
        {            
            return JsonConvert.SerializeObject(envelope);
        }        
    }
}

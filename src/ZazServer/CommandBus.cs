using System.Net;
using System.Net.Http;
using System.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zaz.Server
{
    [ServiceContract]
    public class CommandBus
    {
        [WebGet(UriTemplate = "")]
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage
                       {
                           Content = new StringContent("Endpoint for commands.")
                       };
        }        

        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            ICommandBroker broker = null;
            var body = request.Content.ReadAsString();
            var envelope = JObject.Parse(body);
            var key = envelope["Key"].Value<string>();
            var cmdType = broker.ResolveCommandType(key);
            var cmdReader = envelope["Command"].CreateReader();
            var serializer =new JsonSerializer();
            var cmd = serializer.Deserialize(cmdReader, cmdType);
            broker.Handle(cmd).Wait();
            return new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.Accepted,
                           Content = new StringContent("Command " + key + " accepted")
                       };
        }
    }
}

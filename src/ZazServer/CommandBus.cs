using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zaz.Server
{
    [ServiceContract]
    public class CommandBus
    {
        private readonly ICommandBroker _broker;
        private readonly Conventions _conventions;

        public CommandBus(ICommandBroker broker,
            Conventions conventions)
        {
            _broker = broker;
            _conventions = conventions;
        }

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
            var body = request.Content.ReadAsString();

            var envelope = JObject.Parse(body);
            var key = envelope["Key"].Value<string>();
            var cmdReader = envelope["Command"].CreateReader();

            var cmdType = _conventions.CommandResolver(key);
            if (cmdType == null)
            {                
                throw new HttpException("Command was not found");
            }
            var serializer = new JsonSerializer();
            var cmd = serializer.Deserialize(cmdReader, cmdType);

            _broker.Handle(cmd).Wait();
            return new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.Accepted,
                           Content = new StringContent("Command " + key + " accepted")
                       };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using Microsoft.ApplicationServer.Http.Dispatcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zaz.Server
{
    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CommandsService
    {
        private readonly ICommandBroker _broker;
        private readonly Conventions _conventions;

        public CommandsService(ICommandBroker broker,
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
            if (request.Content.Headers
                .GetValues("Content-Type")
                .Any(x => x == "application/x-www-form-urlencoded"))
            {                
                var body = request.Content.ReadAsString();
                var form = ParseQueryString(body);

                if (!form.ContainsKey("Zaz-Command-Id"))
                {
                    throw CreateApiException("Required value 'Zaz-Command-Id' was not found.");
                }

                var key = form["Zaz-Command-Id"];
                var cmdType = ResolveCommand(key);

                var binder = new CommandBinder();
                var cmd = binder.Build(cmdType, form);

                _broker.Handle(cmd).Wait();

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Accepted,
                    Content = new StringContent("Command " + key + " accepted")
                };
            }
            else
            {
                var body = request.Content.ReadAsString();

                var envelope = JObject.Parse(body);
                var key = envelope["Key"].Value<string>();
                var cmdReader = envelope["Command"].CreateReader();

                var cmdType = ResolveCommand(key);

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

        private Type ResolveCommand(string key)
        {
            var cmdType = _conventions.CommandResolver(key);
            if (cmdType == null)
            {
                throw CreateApiException("Command was not found");
            }

            return cmdType;
        }

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var nvc = HttpUtility.ParseQueryString(query);
            return nvc.Keys
                .Cast<string>()
                .ToDictionary(x => x, x => nvc[x]);
        }

        private static HttpResponseException CreateApiException(string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest, message);
            return new HttpResponseException(resp);
        }
    }
}

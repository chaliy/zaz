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
                .Any(x => x.Key == "Content-Type" 
                    && x.Value
                    .Any(xx => xx == "application/x-www-form-urlencoded")))
            {                
                var body = request.Content.ReadAsString();
                var form = ParseQueryString(body);

                if (!form.ContainsKey("Zaz-Command-Id"))
                {
                    throw CreateApiException("Required value 'Zaz-Command-Id' was not found.");
                }

                var cmdKey = form["Zaz-Command-Id"];
                var cmdType = ResolveCommand(cmdKey);

                var binder = new CommandBinder();
                var cmd = binder.Build(cmdType, form);

                return HandleCommand(cmdKey, cmd);
            }
            else
            {
                var body = request.Content.ReadAsString();

                var envelope = ReadValidCommandEnvelope(body);                
                var cmdKey = envelope["Key"].Value<string>();
                var cmdType = ResolveCommand(cmdKey);
                                
                var cmd = DeserializeCommand(envelope, cmdType);

                return HandleCommand(cmdKey, cmd);
            }            
        }

        private HttpResponseMessage HandleCommand(string cmdKey, object cmd)
        {         
            _broker.Handle(cmd).Wait();            
            return new HttpResponseMessage
                       {
                           StatusCode = HttpStatusCode.Accepted,
                           Content = new StringContent("Command " + cmdKey + " accepted")
                       };
        }

        private static JObject ReadValidCommandEnvelope(string body)
        {
            try
            {
                var envelope = JObject.Parse(body);
                if (envelope["Key"] == null)
                {
                    throw CreateApiException("Required value 'Key' was not found.");                    
                }
                return envelope;

            }
            catch (JsonReaderException ex)
            {
                throw CreateApiException("Problems with deserializing command data. " + ex.Message);
            }            
        }

        private static object DeserializeCommand(JObject envelope, Type cmdType)
        {
            if (envelope["Command"] != null)
            {
                var cmdReader = envelope["Command"].CreateReader();
                try
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize(cmdReader, cmdType);
                }
                catch (JsonReaderException ex)
                {
                    throw CreateApiException("Problems with deserializing command data. " + ex.Message);
                }            
            }
            return Activator.CreateInstance(cmdType);
        }

        private Type ResolveCommand(string key)
        {
            var cmdType = _conventions.CommandResolver(key);
            if (cmdType == null)
            {
                throw CreateApiException("Command " + key + " was not found");
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

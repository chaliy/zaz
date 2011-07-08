using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public CommandsService(ICommandBroker broker) 
            : this(broker, null)
        {
        }

        public CommandsService(ICommandBroker broker,
            Conventions conventions)
        {
            _broker = broker;
            _conventions = conventions ?? new Conventions();
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
            //Debugger.Break();
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
                
                var cmd = BindFormToCommand(form, cmdType);

                return HandleCommand(cmdKey, cmd, new string[0]);
            }
            else
            {
                var body = request.Content.ReadAsString();

                var envelope = ReadValidCommandEnvelope(body);                
                var cmdKey = envelope["Key"].Value<string>();
                var cmdType = ResolveCommand(cmdKey);
                                
                var cmd = DeserializeCommand(envelope, cmdType);
                var tags = ReadTags(envelope);
                return HandleCommand(cmdKey, cmd, tags);
            }            
        }        

        private static object BindFormToCommand(Dictionary<string, string> form, Type cmdType)
        {            
            try
            {
                var binder = new CommandBinder();
                return binder.Build(cmdType, form);                    
            }
            catch (InvalidOperationException ex)
            {
                throw CreateApiException("Problems with binding command data. " + ex.Message);
            }
        }

        private HttpResponseMessage HandleCommand(string cmdKey, object cmd, string[] tags)
        {
            _broker.Handle(cmd, new CommandHandlingContext
                                    {
                                        Tags = tags ?? new string[0]
                                    })
                   .Wait();
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

        private static string[] ReadTags(JObject envelope)
        {
            if (envelope["Tags"] != null)
            {
                var cmdReader = envelope["Tags"].CreateReader();
                try
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<string[]>(cmdReader);
                }
                catch (JsonReaderException ex)
                {
                    throw CreateApiException("Problems with deserializing tags data. " + ex.Message);
                }
            }
            return new string[0];
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
            var cmdType = (_conventions.CommandResolver 
                           ?? DefaultConventions.CommandResolver)(key);
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
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest, message)
                           {
                               Content = new StringContent(message)
                           };
            return new HttpResponseException(resp);
        }        
    }
}

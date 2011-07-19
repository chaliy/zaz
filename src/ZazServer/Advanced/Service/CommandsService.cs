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
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Ui;

namespace Zaz.Server.Advanced.Service
{
    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]    
    public class CommandsService
    {
        private readonly Conventions _conventions;
        
        public CommandsService(Conventions conventions = null)
        {            
            _conventions = conventions ?? new Conventions();            
        }
        
        [WebGet(UriTemplate = "/{*path}")]
        public HttpResponseMessage Get(string path = "index.html")
        {
            return new HttpResponseMessage
                       {
                           Content = UiContent.Get(path)
                       };
        }

        [WebGet(UriTemplate = "MetaList")]
        public IQueryable<CommandMeta> MetaList()
        {
            var registry = (_conventions.CommandRegistry ?? DefaultConventions.CommandRegistry);
            return registry
                .Query()
                .Select(x => new CommandMeta
                            {
                                Key = x.Key,
                                Description = x.Description,
                                Aliases = x.Aliases,
                                Tags = x.Tags,
                                Parameters = x.Parameters
                                              .Select(xx => new CommandMetaParameter
                                                                {
                                                                    Name = xx.Name,
                                                                    Description = xx.Description,
                                                                    Type = xx.Type
                                                                })
                                              .ToArray()
                            });
        }      

        private static object BindFormToCommand(IDictionary<string, string> form, Type cmdType)
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

        [WebInvoke(Method = "POST", UriTemplate = "Legacy")]
        public HttpResponseMessage LegacyPost(HttpRequestMessage req)
        {
            //if (req.Content.Headers
            //    .Any(x => x.Key == "Content-Type"
            //        && x.Value.Any(xx => xx == "application/x-www-form-urlencoded")))
            //{
            // Old legacy version

            var body = req.Content.ReadAsString();
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


        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Post(CommandEnvelope env)
        {            
            var cmdKey = env.Key;
            if (String.IsNullOrWhiteSpace(cmdKey))
            {
                throw CreateApiException("Required value 'Key' was not found.");
            }

            var cmdType = ResolveCommand(cmdKey);
            var cmd = BuildCommand(env, cmdType);
            return HandleCommand(cmdKey, cmd, env.Tags);
        }

        private static object BuildCommand(dynamic env, Type cmdType)
        {
            try
            {
                if (env.Command != null)
                {
                    var serializer = new JsonSerializer();
                    var reader = ((JObject) env.Command).CreateReader();
                    var cmd = serializer.Deserialize(reader, cmdType);
                    return cmd;
                }
            }            
            catch (JsonReaderException ex)
            {
                throw CreateApiException("Problems with deserializing command data. " + ex.Message);
            }

            return Activator.CreateInstance(cmdType);
        }        

        private HttpResponseMessage HandleCommand(string cmdKey, object cmd, string[] tags)
        {
            var broker = (_conventions.CommandBroker ?? DefaultConventions.CommandBroker);
            broker.Handle(cmd, new CommandHandlingContext
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
        
        private Type ResolveCommand(string key)
        {
            var cmdType = (_conventions.CommandRegistry
                           ?? DefaultConventions.CommandRegistry)
                .Query()
                .Where(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Type)
                .FirstOrDefault();

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

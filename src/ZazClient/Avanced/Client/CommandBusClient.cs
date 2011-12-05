﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Client.Avanced.Contract;

namespace Zaz.Client.Avanced.Client
{
    public class CommandBusClient
    {
        private readonly HttpClient _client;

        public CommandBusClient(string url, ZazConfiguration configuration = null)
        {
            var handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = 16777216;
            handler.MaxResponseHeadersLength = 16777216;
            if (configuration != null && configuration.ConfigureHttp != null)
            {
                configuration.ConfigureHttp(handler);                
            }
            //handler.Credentials = new NetworkCredential("test", "test");
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(url);            
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
        }

        public Task<HttpResponseMessage> Post(PostCommandRequest req)
        {
            return _client.PostAsync("", CreateObjectContent<PostCommandRequest>(req));
        }

        public Task<PostScheduledCommandResponse> PostScheduled(PostCommandRequest req)
        {            
            return PostAsync<PostCommandRequest, PostScheduledCommandResponse>("Scheduled", req);                
        }

        public GetScheduledCommandResponse GetScheduled(string id)
        {
            return ReadContentAs<GetScheduledCommandResponse>(_client.Get("Scheduled/" + id + "/").Content);
        }

        private Task<TOut> PostAsync<T, TOut>(string path, T req)
        {
            return _client
                .PostAsync(path, CreateObjectContent<T>(req))
                .ContinueWith(t => {

                    var resp = t.Result;

                    if (!resp.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException("An error occured while sending request. Server response: \r\n" + resp);
                    }

                    return ReadContentAs<TOut>(resp.Content);
                });
        }

        private static T ReadContentAs<T>(HttpContent content) 
        {
            return content.ReadAs<T>(new[] { new JsonNetFormatter() });
        }

        private ObjectContent<T> CreateObjectContent<T>(T input) 
        {
            return new ObjectContent<T>(input, 
                new MediaTypeHeaderValue("application/json"),
                new[] { new JsonNetFormatter() });
        }
    }
}

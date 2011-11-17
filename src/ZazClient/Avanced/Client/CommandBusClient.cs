using System;
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

        public CommandBusClient(string url)
        {
            var handler = new WebRequestHandler();
            handler.MaxRequestContentBufferSize = 16777216;
            handler.MaxResponseHeadersLength = 16777216;
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
                .ContinueWith(t => ReadContentAs<TOut>(t.Result.Content));
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

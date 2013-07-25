using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Client.Avanced.Contract;

namespace Zaz.Client.Avanced.Client
{
    public class ZazServerClient
    {
        private readonly HttpClient _client;

        public ZazServerClient(string url, ZazConfiguration configuration = null)
        {
            var handler = new WebRequestHandler
            {
                MaxRequestContentBufferSize = 16777216,
                MaxResponseHeadersLength = 16777216
            };

            if (configuration != null && configuration.ConfigureHttp != null)
            {
                configuration.ConfigureHttp(handler);
            }

            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(url)
            };

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (configuration != null && configuration.ConfigureDefaultHeaders != null)
            {
                configuration.ConfigureDefaultHeaders(_client.DefaultRequestHeaders);
            }
        }

        public Task<HttpResponseMessage> Post(PostCommandRequest req)
        {
            return _client.PostAsync("", CreateObjectContent(req));
        }

        public Task<PostScheduledCommandResponse> PostScheduled(PostCommandRequest req)
        {
            return PostAsync<PostCommandRequest, PostScheduledCommandResponse>("Scheduled", req);
        }

        public GetScheduledCommandResponse GetScheduled(string id, DateTime token)
        {
            var t = token.ToString("o");
            return ReadContentAs<GetScheduledCommandResponse>(_client.GetAsync("Scheduled/" + id + "/?token=" + t).Result.Content);
        }

        private Task<TOut> PostAsync<T, TOut>(string path, T req)
        {
            return _client.PostAsync(path, CreateObjectContent(req))
                .ContinueWith(t =>
                {
                    var resp = t.Result;

                    if (!resp.IsSuccessStatusCode)
                    {
                        throw new ZazTransportException("An error occurred while sending request.", resp);
                    }

                    return ReadContentAs<TOut>(resp.Content);
                });
        }

        private static T ReadContentAs<T>(HttpContent content)
        {
            return content.ReadAsAsync<T>(new[] { new JsonNetFormatter2() }).Result;
        }

        private static ObjectContent<T> CreateObjectContent<T>(T input)
        {
            return new ObjectContent<T>(input, new JsonNetFormatter2(), new MediaTypeHeaderValue("application/json"));
        }
    }
}

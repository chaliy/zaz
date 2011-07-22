using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZazAbstr.Advanced.Service;

namespace Zaz.Client.Avanced
{
    public class AdvancedCommandBus
    {
        private readonly HttpClient _client;

        public AdvancedCommandBus(string url)
        {
            _client = new HttpClient(url);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Task Post(CommandEnvelope envelope)
        {
            return _client.PostAsync("", new CommandContent(envelope))
                .ContinueWith(x =>
                {
                    if (!x.Result.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException("Command was not successfully posted.");
                    }
                    return;
                });                
        }

        public Task Post2(PostScheduledCommandRequest req)
        {
            return _client.PostAsync("Scheduled", JsonContentExtensions.Create(req))
                .ContinueWith(x =>
                {                    
                    //if (!resp.IsSuccessStatusCode)
                    //{
                    //    throw new InvalidOperationException("Command was not successfully posted.");
                    //}

                    var resp = x.Result.Content.ReadAs<PostScheduledCommandResponse>();
                    var id = resp.Id;
                    Console.WriteLine("Start waiting for execution command " + id);

                    while (true)
                    {                       
                        var resp2 = _client.Get("Scheduled/" + id).Content;
                        var resp3 = resp2.ReadAs<GetScheduledCommandResponse>();

                        Console.WriteLine(resp3.Status);
                    }

                    //resp.Id;
                    return;
                });
        }
    }
}

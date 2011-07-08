using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Zaz.Client.Avanced
{
    public class AdvancedCommandBus
    {
        private readonly HttpClient _client;

        public AdvancedCommandBus(string url)
        {
            _client = new HttpClient(url);
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
    }
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Zaz.Client.Avanced.Client;
using Zaz.Client.Avanced.Contract;

namespace Zaz.Client.Avanced
{
    public class AdvancedCommandBus
    {        
        private readonly CommandBusClient _client;

        public AdvancedCommandBus(string url, ZazConfiguration configuration = null)
        {
            _client = new CommandBusClient(url, configuration);
        }        
        
        public Task<string> Post(CommandEnvelope envelope)
        {
            var req = CreatePostCommandRequest(envelope);
            return _client.Post(req)
                .ContinueWith(x =>
                {                    
                    if (!x.Result.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException("An error occured while sending request. Server response: \r\n" + x.Result);
                    }
                    return x.Result.Content.ReadAsString();
                });                
        }

        public Task PostScheduled(CommandEnvelope envelope)
        {
            var req = CreatePostCommandRequest(envelope);
            return _client.PostScheduled(req)
                .ContinueWith(x =>
                {                    
                    var resp = x.Result;                    
                    var id = resp.Id;

                    WriteTrace("Start waiting for execution command " + id);

                    while (true)
                    {
                        Thread.Sleep(300);

                        var resp2 = _client.GetScheduled(id);

                        switch (resp2.Status)
                        {
                            case ScheduledCommandStatus.Pending:
                                WriteTrace("Command(" + id + ") is still pending.");
                                break;

                            case ScheduledCommandStatus.InProgress:
                                WriteTrace("Command(" + id + ") is in progress.");
                                break;

                            case ScheduledCommandStatus.Success:
                                WriteTrace("Command(" + id + ") completed success.");
                                return;

                            case ScheduledCommandStatus.Failure:
                                WriteTrace("Command(" + id + ") failed.");
                                return;
                        }
                                                    
                    }                                       
                });
        }

        private static PostCommandRequest CreatePostCommandRequest(CommandEnvelope envelope)
        {
            return new PostCommandRequest
            {
                Command = envelope.Command,
                Key = envelope.Key,
                Tags = envelope.Tags
            };
        }        

        private void WriteTrace(string msg)
        {
            System.Diagnostics.Trace.TraceInformation(msg);
        }
    }
}

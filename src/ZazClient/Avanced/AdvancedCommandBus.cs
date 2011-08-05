using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Zaz.Client.Avanced.Contract;

namespace Zaz.Client.Avanced
{
    public class AdvancedCommandBus
    {
        private readonly HttpClient _client;
        private readonly Subject<string> _trace = new Subject<string>();

        public AdvancedCommandBus(string url)
        {
            _client = new HttpClient(url);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IObservable<string> Trace
        {
            get { return _trace; }
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

        public Task PostScheduled(PostScheduledCommandRequest req)
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
                    WriteTrace("Start waiting for execution command " + id);

                    while (true)
                    {
                        var resp2 = _client.Get("Scheduled/" + id).Content.ReadAs<GetScheduledCommandResponse>();

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
                            
                        Thread.Sleep(200);
                    }                                       
                });
        }

        private void WriteTrace(string msg)
        {
            _trace.OnNext(msg);
            System.Diagnostics.Trace.TraceInformation(msg);
        }
    }
}

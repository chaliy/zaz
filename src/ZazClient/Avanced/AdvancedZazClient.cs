using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zaz.Client.Avanced.Client;
using Zaz.Client.Avanced.Contract;

namespace Zaz.Client.Avanced
{
    public class AdvancedZazClient
    {        
        private readonly ZazServerClient _client;

        public AdvancedZazClient(string url, ZazConfiguration configuration = null)
        {
            _client = new ZazServerClient(url, configuration);
        }        
                
        public Task<string> Post(CommandEnvelope envelope)
        {
            var req = CreatePostCommandRequest(envelope);
            return _client.Post(req)
                .ContinueWith(x =>
                {
                    if (!x.Result.IsSuccessStatusCode)
                    {
                        throw new ZazTransportException("An error occured while sending request.", x.Result);
                    }
                    return x.Result.Content.ReadAsString();
                });                
        }

        public Task PostScheduled(CommandEnvelope envelope, IObserver<LogEntry> log)
        {            
            var req = CreatePostCommandRequest(envelope);
            return _client.PostScheduled(req)
                .ContinueWith(x =>
                {                    
                    var resp = x.Result;
                    var id = resp.Id;                    
                    
                    Action<string> traceStatus = m => log.OnNext(new LogEntry
                    {
                        Message = m,
                        Severity = LogEntrySeverity.Trace,
                        Timestamp = DateTime.Now
                    });

                    traceStatus("Start waiting for execution command " + id);

                    var token = DateTime.MinValue;

                    while (true)
                    {
                        var resp2 = _client.GetScheduled(id, token);

                        // Write user log        
                        var serverLog = resp2.Log.OrEmpty().ToList();
                        foreach (var entry in serverLog)
                        {
                            log.OnNext(entry);
                        }

                        // Take maximum available timestamp
                        token = serverLog.Select(xx => xx.Timestamp).Union(new[] { token }).Max();                        
                        
                        switch (resp2.Status)
                        {
                            case ScheduledCommandStatus.Pending:
                                traceStatus("Command(" + id + ") is still pending.");
                                break;

                            case ScheduledCommandStatus.InProgress:
                                traceStatus("Command(" + id + ") is in progress.");
                                break;

                            case ScheduledCommandStatus.Success:
                                traceStatus("Command(" + id + ") completed success.");
                                return;

                            case ScheduledCommandStatus.Failure:
                                traceStatus("Command(" + id + ") failed.");
                                return;
                        }

                        Thread.Sleep(300);       
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
    }
}

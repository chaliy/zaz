using System;
using System.Threading.Tasks;
using Zaz.Client.Avanced;
using Zaz.Client.Avanced.Logging;

namespace Zaz.Client
{
    public class ZazClient
    {
        readonly AdvancedZazClient _underlineClient;

        public ZazClient(string url, ZazConfiguration configuration = null)
        {
            _underlineClient = new AdvancedZazClient(url, configuration);
        }

        /// <summary>
        ///     Post command in sync manner.
        /// </summary>        
        /// <exception cref="Zaz.Client.ZazException">Wraps all errors occurred while sending command</exception>
        [Obsolete("Use PostAsync method instead")]
        public string PostLegacy(object cmd, params string[] tags)
        {
            try
            {
                var posting = _underlineClient.PostAsync(new CommandEnvelope
                {
                    Key = cmd.GetType().FullName,
                    Command = cmd,
                    Tags = tags
                });

                return posting.Result;
            }
            catch (AggregateException aex)
            {
                var ex = aex.Flatten();
                if (ex.InnerExceptions.Count > 1)
                {
                    throw ZazException.CreateDefault(ex);
                }
                var ex2 = ex.GetBaseException();
                throw ZazException.CreateDefault(ex2);
            }
        }

        public Task<string> PostAsync(object cmd, params string[] tags)
        {
            return _underlineClient.PostAsync(new CommandEnvelope
            {
                Key = cmd.GetType().FullName,
                Command = cmd,
                Tags = tags
            }).ContinueWith(task =>
            {
                if (task.Status == TaskStatus.Faulted && task.Exception != null)
                {
                    var aex = task.Exception;

                    var ex = aex.Flatten();
                    if (ex.InnerExceptions.Count > 1)
                    {
                        throw ZazException.CreateDefault(ex);
                    }
                    var ex2 = ex.GetBaseException();
                    throw ZazException.CreateDefault(ex2);
                }

                return task.Result;
            });
        }

        /// <summary>
        /// Post command in sync manner.
        /// This method causing some heavy traffic on checking the scheduled task status. And this method is not 
        /// covered with any tests as they been getting stuck on the massive tests run locally and on the build 
        /// server. This marked it as protected to make possible to write some hacks in case of compatibility issues.
        /// </summary>        
        /// <exception cref="Zaz.Client.ZazException">Wraps all errors occurred while sending command</exception>
        [Obsolete("Use PostAsync method instead.")]
        protected void Post(object cmd, string[] tags = null, IObserver<LogEntry> log = null)
        {
            try
            {
                var posting = _underlineClient.PostScheduled(new CommandEnvelope
                {
                    Key = cmd.GetType().FullName,
                    Command = cmd,
                    Tags = tags
                }, log ?? new ZazLogToSystemTraceAdapter());

                posting.Wait();
            }
            catch (AggregateException aex)
            {
                var ex = aex.Flatten();
                if (ex.InnerExceptions.Count > 1)
                {
                    throw ZazException.CreateDefault(ex);
                }
                var ex2 = ex.GetBaseException();
                throw ZazException.CreateDefault(ex2);
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Zaz.Client.Avanced;

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
        ///     Post command in sync maner.
        /// </summary>        
        /// <exception cref="Zaz.Client.ZazException">Wraps all erros occured while sending command</exception>
        public string Post(object cmd, params string[] tags)
        {
            try
            {
                var posting = _underlineClient.Post(new CommandEnvelope
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
                else
                {
                    var ex2 = ex.GetBaseException();
                    throw ZazException.CreateDefault(ex2);
                }
            }
        }

        public Task PostAsync(object cmd, params string[] tags)
        {
            return _underlineClient.Post(new CommandEnvelope
            {
                Key = cmd.GetType().FullName,
                Command = cmd,
                Tags = tags
            });
        }
    }
}

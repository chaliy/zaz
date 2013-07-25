using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using SampleCommands;
using SampleHandlers;
using Zaz.Client;
using Zaz.Client.Avanced.Logging;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

namespace Zaz.Tests.Integration
{
    public class When_posting_20_commands_through_shared_client
    {
        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/SomeCommands/";

        HttpSelfHostServer _host;
        ZazClient _client;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly),
            });

            _host = new HttpSelfHostServer(config);
            _host.OpenAsync().Wait();

            _client = new ZazClient(URL);

        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _host.CloseAsync().Wait();
        }

        [Test]
        public void Should_successfully_send_all_commands_and_recieve_the_result()
        {
            const int concurrentCalls = 20;

            var allTasks = new List<Task>();

            for (var i = 0; i < concurrentCalls; i++)
            {
                var local = i;
                var task = Task.Factory.StartNew(() =>
                {
                    var logAdapter = new ZazLogToStringAdapter();
                    var cmd = new PrintMessage { Message = "Hello world #" + (local + 1) };
                    _client.Post(cmd, new[] { "Tag1" }, logAdapter);
                    var result = logAdapter.ToString();
                    return result;
                });

                allTasks.Add(task);
            }

            var allTasksArray = allTasks.ToArray();
            Task.WaitAll(allTasksArray, TimeSpan.FromMinutes(5));

            var counters = new
            {
                RanToCompletion = allTasks.Count(task => task.Status == TaskStatus.RanToCompletion),
                Canceled = allTasks.Count(task => task.Status == TaskStatus.Canceled),
                Created = allTasks.Count(task => task.Status == TaskStatus.Created),
                Faulted = allTasks.Count(task => task.Status == TaskStatus.Faulted),
                Running = allTasks.Count(task => task.Status == TaskStatus.Running),
                WaitingForActivation = allTasks.Count(task => task.Status == TaskStatus.WaitingForActivation),
                WaitingForChildrenToComplete = allTasks.Count(task => task.Status == TaskStatus.WaitingForChildrenToComplete),
                WaitingToRun = allTasks.Count(task => task.Status == TaskStatus.WaitingToRun),
            };

            counters.RanToCompletion.Should().Be(concurrentCalls, "All tasks should be completed in reasonable period of time", counters);
        }
    }
}

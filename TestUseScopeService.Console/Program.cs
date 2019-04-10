using Common;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using System;
using System.Threading.Tasks;

namespace TestUseScopeService.Console
{
    class Program
    {
        const string SbConnectionString = "CONNECTION_STRING_HERE";
        const string SbBaseUrl = "SB_BASE_URL";

        static async Task Main(string[] args)
        {
            var bus =
                Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    var host = cfg.Host(SbConnectionString, sbcfg => { });
                    EndpointConvention.Map<ITestMessage>(new Uri($"{SbBaseUrl}test-queue"));
                });

            await bus.StartAsync();

            do
            {
                await bus.Publish<ITestMessage>(new TestMessage { TestField = "Hello, World." });

            } while (System.Console.ReadLine() != "exit");

            await bus.StopAsync();
        }
    }

    public class TestMessage : ITestMessage
    {
        public string TestField { get; set; }
    }

}

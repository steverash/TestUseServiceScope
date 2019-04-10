using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using GreenPipes;
using MassTransit;
using MassTransit.AspNetCoreIntegration;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestUseServiceScope.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(provider => 
                Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    cfg.UseServiceScope(provider);

                    var host = cfg.Host(Configuration.GetValue<string>("ServiceBus:ConnectionString"), sbcfg => { });

                    cfg.ReceiveEndpoint("test-queue", e =>
                    {
                        e.UseMessageRetry(c => c.Immediate(5));
                        e.Consumer<TestConsumer>(provider);
                    });
                }),
                x =>
                {
                    x.AddConsumer<TestConsumer>();
                }
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }

    public class TestConsumer : IConsumer<ITestMessage>
    {
        private readonly ILogger<TestConsumer> _logger;

        public TestConsumer(ILogger<TestConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ITestMessage> context)
        {
            _logger.LogDebug("Message Consumed!!!!");
        }
    }

}

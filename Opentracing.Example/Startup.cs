using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using Jaeger;
using Jaeger.Samplers;
using Jaeger.Reporters;
using OpenTracing.Util;
using Jaeger.Senders;

namespace Opentracing.Example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(build=>{
                build.AddConsole();
            });

            services.AddApplicationInsightsTelemetry();

            services.AddOpenTracing();   

            services.AddDbContext<Opentracing.DataAccess.TracingDbContext>();

            services.AddSingleton<ITracer>(serviceProvider => {
                string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                loggerFactory.AddConsole();
                 var remoteReporter = new RemoteReporter.Builder()
                                        //.WithSender(new UdpSender("jaeger",6831,0))
                                        .WithLoggerFactory(loggerFactory)
                                        .Build();
                   var loggingReporter = new LoggingReporter(loggerFactory);
                var tracer = new Tracer.Builder(serviceName)
                              
                              .WithSampler(new ConstSampler(true))
                              .WithReporter(new CompositeReporter(remoteReporter,loggingReporter))
                              .Build();
                GlobalTracer.Register(tracer);
                return tracer;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseMvc();
        }
    }
}

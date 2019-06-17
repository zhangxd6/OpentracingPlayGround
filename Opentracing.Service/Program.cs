using System;
using Jaeger;
using Jaeger.Samplers;
using Jaeger.Reporters;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using Jaeger.Senders;
using OpenTracing.Tag;
using System.Collections.Generic;
using OpenTracing.Propagation;

namespace Opentracing.Service
{
  class Program
  {
    async static Task Main(string[] args)
    {
      var loggerFactory = new LoggerFactory().AddConsole();

            var loggingReporter = new LoggingReporter(loggerFactory);
            var remoteReporter = new RemoteReporter.Builder()
                .WithLoggerFactory(loggerFactory)
                .Build();
            var tracer = new Tracer.Builder("Server")
                .WithLoggerFactory(loggerFactory)
                .WithSampler(new ConstSampler(true))
                .WithReporter(new CompositeReporter(loggingReporter, remoteReporter))
                .Build();
            var hclient = new HttpClient();
      for (int i = 0; i < 10; i++)
      {
        using (var scope = tracer.BuildSpan("Init").StartActive(true))
        {
          scope.Span.SetBaggageItem("index", $"{i}")
                    .SetTag(Tags.SpanKind,Tags.SpanKindClient)
                    .SetTag(Tags.HttpMethod,"GET");
          var dictionary = new Dictionary<string, string>();
          tracer.Inject(scope.Span.Context,BuiltinFormats.HttpHeaders,new TextMapInjectAdapter(dictionary));
          hclient.DefaultRequestHeaders.Clear();
          foreach(var entry in dictionary)
          {
            hclient.DefaultRequestHeaders.Add(entry.Key,entry.Value);
          }
          var result = await hclient.GetAsync("http://localhost:5000/api/value");
          System.Threading.Thread.Sleep(1000);
          
        }
      }
    }
  }
}

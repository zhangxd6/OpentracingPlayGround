
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Propagation;
using OpenTracing.Tag;

[Route("api/value")]

public class ValueController : Controller
{
  private readonly ITracer _tracer;
  private readonly ILogger _logger;

  public ValueController(ITracer tracer,ILoggerFactory loggerfactory)
  {
    _tracer = tracer;
    _logger = loggerfactory.CreateLogger<ValueController>();
  }

  [HttpGet]
  public string Get()
  {   
     var headers = Request.Headers.ToDictionary(k => k.Key, v => v.Value.First());

    using (var scope = _tracer.BuildSpan("ASS").StartActive(true))
    {
      _logger.LogDebug($"Child:{scope.Span.Context.SpanId}:{scope.Span.Context.TraceId}");
      var result = "mew ";
      scope.Span.SetTag("hello-to", result);

      return result;

    }
  }

  public static IScope StartServerSpan(ITracer tracer, IDictionary<string, string> headers, string operationName, ILogger logger)
  {
    ISpanBuilder spanBuilder;
    try
    {
      ISpanContext parentSpanCtx = tracer.Extract(BuiltinFormats.HttpHeaders, new TextMapExtractAdapter(headers));

      logger.LogDebug($"{string.Join(",",headers.Select(kv=> $"{kv.Key}:{kv.Value}").ToArray())}");

      spanBuilder = tracer.BuildSpan(operationName);
      if (parentSpanCtx != null)
      {
        logger.LogDebug($"parnet:{parentSpanCtx.SpanId}:{parentSpanCtx.TraceId}");
        spanBuilder = spanBuilder.AsChildOf(parentSpanCtx);
        
      }
    }
    catch (Exception)
    {
      spanBuilder = tracer.BuildSpan(operationName);
    }

    // TODO could add more tags like http.url
    // return spanBuilder.WithTag(Tags.SpanKind, Tags.SpanKindServer).StartActive(true);
  }
}
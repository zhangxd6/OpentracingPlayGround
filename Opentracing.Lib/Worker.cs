using System;
using System.Threading.Tasks;
using OpenTracing;
using OpenTracing.Util;

namespace Opentracing.Lib
{
  public class Worker
  {
    private readonly ITracer _tracer;
    public Worker() : this(null)
    {

    }
    public Worker(ITracer tracer)
    {
      if (tracer is null)
      {
        if (GlobalTracer.Instance != null)
        {
          _tracer = GlobalTracer.Instance;
        }
        else
        {
          throw new ArgumentNullException(nameof(tracer));
        }
      }
      else
      {
        _tracer = tracer;
      }
    }
    public async Task SomeWork()
    {
      using (var scope = _tracer.BuildSpan("Worker").StartActive(true))
      {
          scope.Span.SetTag("Worker","Doing work");

         
          await Task.CompletedTask;
      }
    }
  }
}

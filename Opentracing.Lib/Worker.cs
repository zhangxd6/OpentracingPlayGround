using System;
using System.Threading.Tasks;
using Opentracing.DataAccess;
using OpenTracing;
using OpenTracing.Util;

namespace Opentracing.Lib
{
  public class Worker
  {
    private readonly ITracer _tracer;
    private TracingDbContext _db;

    public Worker(Opentracing.DataAccess.TracingDbContext db) : this(null,db)
    {

    }
    public Worker(ITracer tracer, Opentracing.DataAccess.TracingDbContext db)
    {
      _db = db;
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
        scope.Span.SetTag("Worker", "Doing work");


        _db.Persons.Add(new Opentracing.DataAccess.Person()
        {
          FirstName = "Joe",
          LastName = "Doe"
        });
        await _db.SaveChangesAsync();
      }
      await Task.CompletedTask;
    }
  }
}


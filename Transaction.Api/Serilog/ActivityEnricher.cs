using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Transaction.Api.Serilog
{
    public class ActivityEnricher : ILogEventEnricher
    {
        private const string SpanId = "SpanId";
        private const string ParentId = "ParentId";
        private const string TraceId = "TraceId";

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            Activity activity = Activity.Current;

            if (activity is null)
            {
                return;
            }

            logEvent.AddOrUpdateProperty(new LogEventProperty(SpanId, new ScalarValue(activity.SpanId)));
            logEvent.AddOrUpdateProperty(new LogEventProperty(ParentId, new ScalarValue(activity.ParentSpanId)));
            logEvent.AddOrUpdateProperty(new LogEventProperty(TraceId, new ScalarValue(activity.TraceId)));
        }
    }
}

using System;
using Serilog;
using Serilog.Configuration;
using Transaction.Api.Serilog;

namespace Transaction.Api.Extensions
{
    public static class ActivityEnricherExtensions
    {
        public static LoggerConfiguration WithActivityEnricher(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With<ActivityEnricher>();
        }
    }
}

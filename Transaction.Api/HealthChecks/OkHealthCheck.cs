using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Transaction.Api.HealthChecks
{
    public class OkHealthCheck : IHealthCheck
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly string _Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        public OkHealthCheck(IWebHostEnvironment webHostEnvironment)
        {
            this._webHostEnvironment = webHostEnvironment;
        }
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                string info = $"Environment={this._webHostEnvironment.EnvironmentName} Version={OkHealthCheck._Version}";
                return Task.FromResult(
                 HealthCheckResult.Healthy($"OK. {info}"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    new HealthCheckResult(context.Registration.FailureStatus,
                    $"Error. {ex.Message}",
                    ex));
            }


        }

    }
}

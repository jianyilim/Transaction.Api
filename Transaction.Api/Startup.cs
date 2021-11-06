using System;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag.Examples;
using Transaction.Api.Extensions;
using Transaction.Api.HealthChecks;

namespace Transaction.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new MediaTypeApiVersionReader("version");
            });
            services.AddControllers();

            services.AddExampleProviders(Assembly.GetEntryAssembly());

            services.AddOpenApiDocument((config, provider) =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Transaction API";
                    document.Info.Description = $"Transaction API v1";
                    document.Info.TermsOfService = "None";
                };

                config.AddExamples(provider);
            });
            string connectionString = this.Configuration.GetConnectionString("Database");
            // Health checks
            HealthChecksConfigurations healthChecksConfigurations = this.Configuration.BindObject(new HealthChecksConfigurations());

            TimeSpan timeout = TimeSpan.FromSeconds(healthChecksConfigurations.TimeoutInSeconds);
            services.AddHealthChecks()
                .AddCheck<OkHealthCheck>("OK checking", HealthStatus.Unhealthy, timeout: timeout, tags: new[] { "ok", "ready", "live" })
                .AddSqlServer(connectionString, name: "Database connection", failureStatus: HealthStatus.Unhealthy, timeout: timeout, tags: new[] { "ready", "live" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("Application={Application} started. Version={Version} Environment={Environment}",
                env.ApplicationName,
                Assembly.GetEntryAssembly().GetName().Version,
                env.EnvironmentName);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(); // serve OpenAPI/Swagger documents
                app.UseSwaggerUi3(); // serve Swagger UI
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Adding endpoint of health check for the health check ui in UI format
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                // Adding endpoint of ready
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                // Adding endpoint of simple Ok
                endpoints.MapHealthChecks("/health/ok", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains("ok"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}

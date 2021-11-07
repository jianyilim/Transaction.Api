using System;
using System.IO;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Transaction.Api.Extensions;
using Transaction.Api.Filters;
using Transaction.Api.HealthChecks;
using Transaction.Domain.Transactions;
using Transaction.Domain.UnitOfWorks;
using Transaction.Infrastructure.UnitOfWorks;

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
            services.AddControllers(options =>
                {
                    options.Filters.Add<ExceptionHttpResponseHandlingFilter>();
                })
                .AddNewtonsoftJson(opt => opt.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transaction API", Version = "v1" });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });

            services.AddSwaggerGenNewtonsoftSupport();

            string connectionString = this.Configuration.GetConnectionString("Database");

            // Add Db context.
            services.AddDbContext<TransactionDbContext>(
            options =>
                options.UseSqlServer(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ITransactionImportService, TransactionImportService>();

            // Health checks.
            HealthChecksConfigurations healthChecksConfigurations = this.Configuration.BindObject(new HealthChecksConfigurations());

            TimeSpan timeout = TimeSpan.FromSeconds(healthChecksConfigurations.TimeoutInSeconds);
            services.AddHealthChecks()
                .AddCheck<OkHealthCheck>("OK checking", HealthStatus.Unhealthy, timeout: timeout, tags: new[] { "ok", "ready", "live" })
                .AddSqlServer(connectionString, name: "Database connection", failureStatus: HealthStatus.Unhealthy, timeout: timeout, tags: new[] { "ready", "live" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, TransactionDbContext transactionDbContext)
        {
            logger.LogInformation("Application={Application} started. Version={Version} Environment={Environment}",
                env.ApplicationName,
                Assembly.GetEntryAssembly().GetName().Version,
                env.EnvironmentName);

            if (env.IsDevelopment())
            {
                transactionDbContext.Database.EnsureCreated();
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); 
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "Transaction API V1");
                    c.DefaultModelExpandDepth(1);
                });
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

        private IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Domain.Transactions.Transaction>(nameof(Domain.Transactions.Transaction));

            return builder.GetEdmModel();
        }
    }
}

using System;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire_API_SQLite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var options = new SQLiteStorageOptions();
            options.QueuePollInterval = TimeSpan.FromSeconds(30);

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSQLiteStorage(Configuration.GetConnectionString("Default"), options));

            services.AddHangfireServer();
            
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            var option = new BackgroundJobServerOptions { 
                WorkerCount = 1
            };
            var dashboardOptions = new DashboardOptions
            {
                DashboardTitle = "TITOLO DASHBOARD",
                DisplayStorageConnectionString = false
            };

            app.UseHangfireServer(option);
            app.UseHangfireDashboard("/hangfire", dashboardOptions);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

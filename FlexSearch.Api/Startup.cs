using System;
using System.IO;
using System.Net.Mime;
using Core.Models;
using Core.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SearchApi.Services;

namespace SearchApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            CreateRoot();
            ConfigurationService.SetDefault();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRouting();
            services.AddApplicationInsightsTelemetry();
            services.AddAllServices(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile($"{AppDomain.CurrentDomain.BaseDirectory}Logs/" + "{Date}.log");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void CreateRoot()
        {
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}users/root.json"))
            {
                FileOperations.CheckOrCreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}users/");
                FileOperations.WriteObjectToFile($"{AppDomain.CurrentDomain.BaseDirectory}users/root.json",
                    new UserModel
                    {
                        Database = "all",
                        UserName = "root",
                        Password = "1234"
                    });
            }
        }
    }
}
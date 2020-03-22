using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageManipulationApi.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ImageManipulationApi.Service;
using ImageManipulationApi.Repository;
using ImageManipulationApi.Hubs;

namespace ImageManipulationApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Allow cross-origin-request
            services.AddCors();

            // We only need controller support as we will not be using views
            services.AddControllers();

            // Add SignalR to be able to send real-time notification from app<->api
            services.AddSignalR();
            
            // DI database context with desired DbConnection
            services.AddDbContext<ApiDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("LocalDb")));
           
            // DI HttpClients into service to avoid "Socket Exhaustion Exception"
            services.AddHttpClient<IPostalService, PostalService>(
                options =>
                {
                    options.BaseAddress = new Uri("https://api.bring.com/shippingguide/api/postalCode.json");
                    options.DefaultRequestHeaders.Add("X-Bring-Client-URL", "https://localhost:44373");
                });

            // DI on required services
            services.AddTransient<IImageManipulationRepository, ImageManipulationRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseCors(options => {
                options.WithOrigins("https://localhost:44355")
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader();
                options.WithOrigins("https://localhost:44355")
                    .AllowCredentials();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ImageManipulationHub>("/imageManipulationHub");
            });
        }
    }
}

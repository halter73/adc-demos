using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HorseRace.Hubs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HorseRace
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
            services.AddMvc();
            services.AddSingleton<HorseRacer>();
            services.AddSingleton<IHostedService>(sp => sp.GetService<HorseRacer>());
            services.AddSingleton<IHorseRaceHandler, HorseRaceHandler>();
            services.AddSignalR(options =>
            {
                options.JsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime applicationLifetime,
            HorseRacer horseRacer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSignalR(routeBuilder =>
            {
                routeBuilder.MapHub<RaceHub>("race");
            });

            app.UseMvc();
        }
    }
}

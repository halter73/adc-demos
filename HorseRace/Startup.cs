using System;
using System.Threading.Tasks;
using HorseRace.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<IHorseRaceHandler, HorseRaceHandler>();

            services.AddSockets();
            services.AddEndPoint<RawEndPoint>();

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
            HorseRacer horseRacer,
            RawEndPoint rawEndPoint)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSockets(routes =>
            {
                routes.MapEndPoint<RawEndPoint>("raw");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<RaceHub>("race");
            });

            _ = StartRaces(horseRacer, rawEndPoint, applicationLifetime);

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }

        private async Task StartRaces(HorseRacer horseRaceService, RawEndPoint rawEndPoint, IApplicationLifetime applicationLifetime)
        {
            try
            {
                await Task.WhenAll(
                    horseRaceService.RunRaces(applicationLifetime.ApplicationStopping),
                    rawEndPoint.BroadcastPositionsAsync());

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{nameof(HorseRacer)} exited unexpectedly: {ex}");
            }
        }
    }
}

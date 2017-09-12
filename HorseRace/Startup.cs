using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSockets();
            services.AddEndPoint<RawEndPoint>();
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

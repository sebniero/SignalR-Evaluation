using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalREvaulation.Contracts.Models;
using SignalRWebAPI.Hubs;
using SignalRWebAPI.LocalStorage;

namespace SignalRWebAPI
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
            services.AddSingleton<IPersonService, PersonService>();

            services.AddSignalR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CreateDataDumps(app);

            app.UseSignalR(builder =>
            {
                builder.MapHub<PersonHub>("/hub/person");
            });
            app.UseMvc();
        }

        private static void CreateDataDumps(IApplicationBuilder app)
        {
            var personService = app.ApplicationServices.GetService<IPersonService>();

            personService.AddPerson(new Person
            {
                BirthDate = new DateTime(2018,1,1).ToShortDateString(),
                BodySize = "1.80m",
                Name = "Johann"
            });

            personService.AddPerson(new Person
            {
                BirthDate = new DateTime(2017, 1, 1).ToShortDateString(),
                BodySize = "1.71m",
                Name = "Franz"
            });
        }
    }
}

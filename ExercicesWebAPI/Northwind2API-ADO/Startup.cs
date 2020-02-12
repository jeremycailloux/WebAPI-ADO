using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Northwind2API_ADO.Data;

namespace Northwind2API_ADO
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
            services.AddControllers();
            // Cr�e un singleton donnant acc�s � l'ensemble des param�tres de l'appli
            services.AddSingleton<IConfiguration>(Configuration);
            // Enregistre la classe de contexte de donn�es comme service
            services.AddTransient<Northwind2Context>(); // Ajouter le contexte � la liste des services de l�application, afin qu�on puisse l�utiliser dans les contr�leurs gr�ce au m�canisme d�injection de d�pendance (AddTransient = conteneur IOC).
        } // On enregistre le context dans al liste des services

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

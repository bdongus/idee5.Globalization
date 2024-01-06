using System;
using System.IO;
using idee5.Common;
using idee5.Common.Data;
using idee5.Globalization.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using idee5.Globalization.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using idee5.Globalization.Configuration;

namespace idee5.Globalization.Web {
    public class Startup(IConfiguration configuration) {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // configure the parlance options
            services.Configure<LocalizationParlanceOptions>(o => {
                o.Customer = null;
                o.Industry = null;
            });
            // add the EF Core localization implementation
            services.AddEFCoreLocalization(o => o.UseSqlite(new SimpleDB3ConnectionStringProvider().GetConnectionString("idee5.Resources.db3")));
            // add the localization controllers
            services.AddMvc().AddLocalizationControllers();
            // configure the authorization policy
            services.AddAuthorizationBuilder()
                .AddPolicy("CommandPolicy", p => p.RequireAssertion(_ => true))
                .AddPolicy("QueryPolicy", p => p.RequireAssertion(_ => true));

            // Register the Swagger services
            services.AddOpenApiDocument(config => {
                config.Version = "v1";
                config.Title = "idee5 globalization api";
                config.Description = "Web api to access and modify localized resources.";
                config.PostProcess = doc => {
                    doc.Info.Contact = new NSwag.OpenApiContact() {
                        Email = "bernd.dongus@idee5.com",
                        Name = "Bernd Dongus",
                        Url = "https://www.idee5.com"
                    };
                };
            });
            services.RegisterHandlers(typeof(IQueryHandlerAsync<,>));
            services.RegisterHandlers(typeof(ICommandHandlerAsync<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            ArgumentNullException.ThrowIfNull(env);

            string baseDir;

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                // App_Data in the project directory
                baseDir = env.ContentRootPath;
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                // App_Data in wwwroot
                baseDir = env.WebRootPath;
            }
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(baseDir, "App_Data"));
            app.UseStaticFiles();

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi(options => options.Path = "/openapi");
            app.UseReDoc(options => options.Path = "/redoc");
            app.UseHttpsRedirection();
            app.UseRouting();
            // needed for the generated controllers
            app.UseAuthorization();
            app.UseEndpoints(ep => ep.MapControllers());
        }
    }
}

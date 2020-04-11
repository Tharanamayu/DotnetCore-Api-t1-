using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using packt_webapp.Dtos;
using packt_webapp.Entities;
using packt_webapp.Middlewares;
using packt_webapp.Repositories;
using packt_webapp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.IO;

namespace packt_webapp
{


    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables();
            env.ConfigureNLog("nlog.config");

            Configuration = builder.Build();
        }




        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddDbContext<PacktDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISeedDataService, SeedDataService>();
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "My First WebAPI", Version = "v1" });
            });
            
            services.AddMvc(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                config.InputFormatters.Add(new XmlSerializerInputFormatter());
            });
            services.AddApiVersioning(
            config =>
            {
                config.ReportApiVersions = true;
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");

            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Error);
            //add Nlog to ASP.NET Core
            loggerFactory.AddNLog();

            //add NLog.web
            app.AddNLogWeb();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/plain";
                        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (errorFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500, errorFeature.Error, errorFeature.Error.Message);
                        }

                        await context.Response.WriteAsync("There was an error");
                    });
                });
            }
            

            if (env.IsEnvironment("MyEnvironment"))
            {
                app.UseCustomMiddleware();
            }

            //app.UseApiVersioning();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //enabling the swagger middleware
            app.UseSwagger();
            //enabling the swagger middleware
            app.UseSwaggerUI(config => {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "My first WebAPI");
            });
            
            AutoMapper.Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<Customer, CustomerDto>().ReverseMap();
                mapper.CreateMap<Customer, CustomerCreateDto>().ReverseMap(); 
                mapper.CreateMap<Customer, CustomerUpdateDto>().ReverseMap();
            });
            app.AddSeedData();
            app.UseMvc();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using packt_webapp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace packt_webapp.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
        public static async void AddSeedData(this IApplicationBuilder app)
        {
            var seedDataService = app.ApplicationServices.GetRequiredService<ISeedDataService>();
            await seedDataService.EnsureSeedData();
        }
    }
}

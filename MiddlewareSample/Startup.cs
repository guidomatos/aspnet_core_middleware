using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiddlewareSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("Middleware demo");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<EnvironmentMiddleware>();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                var myTimer = System.Diagnostics.Stopwatch.StartNew();
                logger.LogInformation($"--==>> Beginning request in {env.EnvironmentName} environment");

                await next();

                logger.LogInformation($"--==>> Request completed in {myTimer.ElapsedMilliseconds}ms");
            });

            app.MapWhen(context => context.Request.Headers["User-Agent"].Contains("Apple-iPhone"), iPhoneRoute);

            app.Map("/stuff", a => a.Run(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Here is your stuff");
            }));

            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Hello World!");
            });

        }

        private void iPhoneRoute(IApplicationBuilder app)
        {;
            //throw new NotImplementedException();
        }
    }
}

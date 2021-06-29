using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Middleware;
using API.SignalR;
using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Extension methods.
            services.AddAplicationServices(_config);
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Custom middleware.
            app.UseMiddleware<ExceptionMiddleware>();

            //Add security parameters to header to get an A on securityheaders.com site
            app.UseXContentTypeOptions();
            //Browser do not send any referrer information
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            //Cross site scripting protection
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            //Prevent to use application on an iframe somewhere else
            app.UseXfo(opt => opt.Deny());
            //CSP = content security policy
            //UseCspReportOnly allows to see the different errors on dev tools from browser
            //UseCsp blocks everything that is wrong 
            app.UseCsp(opt => opt 
                //Only allow https
                .BlockAllMixedContent()
                //Allow only from domain
                .StyleSources(s => s.Self().CustomSources("https://fonts.googleapis.com"))
                .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources("https//res.cloudinary.com"))
                .ScriptSources(s => s.Self().CustomSources("sha256-6ys35OdahF1VX2f8hEC+bVxe16U7OAggF5DcAdCzIwM="))
            );

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }
            else 
            {
                //Create a middleware to add property to http header
                //max-age=one yeay
                app.Use(async (context, next) => 
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
                    //Call nex piece of middleware
                    await next.Invoke();
                });
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            //Support for serving static files
            //useDefaultFiles is going to look for anything inside wwwroot folder that is called index.html
            app.UseDefaultFiles();
            //Serve static files from wwwroot folder
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            //First authenticate before authorize.
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}

using Application.Activities;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Application.Interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddAplicationServices(this IServiceCollection services, IConfiguration config)
        {
            //Extension method to maintain Startup class tidy.

            //Validating fields in models.
            services.AddControllers(opt => 
            {
                //Ensuring that every single endpoint in API requires authentication
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(config => 
            {
                //Specifying the class that contains the rules.
                config.RegisterValidatorsFromAssemblyContaining<Create>();
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            //AddDbContext allows to use a Database
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            //AddCors allows the API to be consumed from different domains.
            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy => {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithOrigins("http://localhost:3000");
                });
            });
            //AddMediatR allows to use CQRS pattern.
            services.AddMediatR(typeof(List.Handler).Assembly);
            //AddAutoMapper allows to map properties from one object to another.
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            //Object created once per scoped request
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            //Bind values from appsettings.json.Cloudinary to CloudinarySettings class and add to DPI container
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddSignalR();

            return services;
        }
    }
}
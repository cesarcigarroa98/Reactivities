using System.Text;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, 
            IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt => 
            {
                //Password must contain numbers, signs, uppercase and lowercase
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>();

            //Needs to be the same key that was created with token.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

            //Register authentication services.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt => 
                {
                    //How do we want API to validate that token is valid
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //Validate the super secret key generated on server
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            //This policy is used as an attribute on Activities controller
            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("IsActivityHost", policy => {
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });
            //Object is created in every new request
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
            //Inject TokenService class.
            services.AddScoped<TokenService>();

            return services;
        }
    }
}
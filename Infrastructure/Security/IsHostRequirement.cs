using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security
{
    //This is an authorization policy 
    //Logic inside these clases will be summoned when using Authorization attribute on endpoints
    //The goal is to identify if an user is host of certain activity

    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            //Get user id from token
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            //user is not authorized
            if (userId == null) return Task.CompletedTask;

            //Activity id from the route parameters
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());
            
            //We use .Result because we are not using await 
            var attendee = _dbContext.ActivityAttendees
                //This eliminates info from memory. Without this, no attendees are loaded after editing and getting
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.AppUserId == userId && x.ActivityId == activityId)
                .Result;

            if (attendee == null) return Task.CompletedTask;

            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
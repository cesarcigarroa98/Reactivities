using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Persistence;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Domain;

namespace Application.Activities
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccesor;
            public Handler(DataContext context, IUserAccessor userAccesor)
            {
                _userAccesor = userAccesor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                //Get activity 
                var activity = await _context.Activities
                    .Include(a => a.Attendees).ThenInclude(u => u.AppUser)
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (activity == null) return null;

                //Find user by user name
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccesor.GetUserName());

                if (user == null) return null;

                var hostUsername = activity.Attendees.FirstOrDefault(x => x.IsHost)?.AppUser?.UserName;

                //Verify if user is attending activity
                var attendance = activity.Attendees.FirstOrDefault(x => x.AppUser.UserName == user.UserName);

                //Attendee is the host
                if (attendance != null && hostUsername == user.UserName)
                {
                    //If he abandon activiy, then this one is cancelled
                    activity.IsCancelled = !activity.IsCancelled;
                }

                //Normal attendee
                if (attendance != null && hostUsername != user.UserName)
                {
                    //Remove from attendee list
                    activity.Attendees.Remove(attendance);
                }

                //Attendance not going to activity
                if (attendance == null)
                {
                    //Create new attendance
                    attendance = new ActivityAttendee
                    {
                        AppUser = user,
                        Activity = activity,
                        IsHost = false
                    };

                    activity.Attendees.Add(attendance);
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem updating attendance");
            }
        }
    }
}
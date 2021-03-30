using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        //Middleware class to validate properties in object.
        //Use command class as type because there is where the properties live.
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                //Rules live inside ActivityValidator class.

                //This rule applies for the Activity parameter.
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Activities.Add(request.Activity);

                //Returns an number of entries added to DB.
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Fail to create activity");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
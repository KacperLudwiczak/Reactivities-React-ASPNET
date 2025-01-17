using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

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
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .Include(attendees => attendees.Attendees).ThenInclude(user => user.AppUser)
                    .SingleOrDefaultAsync(item => item.Id == request.Id);
                if (activity == null) return null;
                var user = await _context.Users.FirstOrDefaultAsync(item => item.UserName == _userAccessor.GetUsername());
                if (user == null) return null;
                var hostUsername = activity.Attendees.FirstOrDefault(item => item.IsHost)?.AppUser.UserName;
                var attendance = activity.Attendees.FirstOrDefault(item => item.AppUser.UserName == user.UserName);
                if (attendance != null && hostUsername == user.UserName)
                    activity.IsCancelled = !activity.IsCancelled;
                if (attendance != null && hostUsername != user.UserName)
                    activity.Attendees.Remove(attendance);
                if (attendance == null)
                {
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
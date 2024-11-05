using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
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
                var user = await _context.Users
                    .Include(photos => photos.Photos)
                    .FirstOrDefaultAsync(item => item.UserName == _userAccessor.GetUsername());
                var photo = user.Photos.FirstOrDefault(item => item.Id == request.Id);
                if (photo == null) return null;
                var currentMain = user.Photos.FirstOrDefault(item => item.IsMain);
                if (currentMain != null) currentMain.IsMain = false;
                photo.IsMain = true;
                var success = await _context.SaveChangesAsync() > 0;
                if (success) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Problem setting main photo");
            }
        }
    }
}
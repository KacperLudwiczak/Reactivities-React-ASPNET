using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Photo>>
        {

            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _photoAccessor = photoAccessor;
            }
            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
                var user = await _context.Users.Include(photos => photos.Photos)
                    .FirstOrDefaultAsync(item => item.UserName == _userAccessor.GetUsername());
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.PublicId
                };
                if (!user.Photos.Any(item => item.IsMain)) photo.IsMain = true;
                user.Photos.Add(photo);
                var result = await _context.SaveChangesAsync() > 0;
                if (result) return Result<Photo>.Success(photo);
                return Result<Photo>.Failure("Problem adding photo");
            }
        }
    }
}
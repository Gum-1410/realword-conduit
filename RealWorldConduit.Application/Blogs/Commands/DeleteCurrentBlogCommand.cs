using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Common;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public class DeleteCurrentBlogCommand : IRequestWithBaseResponse
    {
        // TODO : Implement Validation Later
        public string Slug { get; set; }
    }

    internal class DeleteBlogCommandHandler : IRequestWithBaseResponseHandler<DeleteCurrentBlogCommand>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public DeleteBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task<BaseResponseDTO> Handle(DeleteCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = await _dbContext.Blogs.FirstOrDefaultAsync(x => x.Title.Equals(request.Slug) && x.AuthorId == _currentUser.Id, cancellationToken);

            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Slug} title is not found!");
            }

            _dbContext.Blogs.Remove(blog);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO
            {
                Code = HttpStatusCode.NoContent,
                Message = $"Successfully delete {request.Slug} blog",
            };
        }
    }
}

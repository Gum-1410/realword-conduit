using MediatR;
using Microsoft.EntityFrameworkCore;
using RealWorldConduit.Domain.Entities;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public record LikeBlogUpsertCommand(string Title) : IRequest;
    internal class UnlikeBlogCommandHandler : IRequestHandler<LikeBlogUpsertCommand>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;
        private readonly IMediator _mediator;

        public UnlikeBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser, IMediator mediator)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task Handle(LikeBlogUpsertCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = (Guid)_currentUser.Id;

            var blog = await _dbContext.Blogs
                            .Include(x => x.FavoriteBlogs)
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title), cancellationToken);

            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Title} title is not found!");
            }

            if (!blog.FavoriteBlogs.Any(x => x.FavoritedById == currentUserId))
            {
                _dbContext.FavoriteBlogs.Add(new FavoriteBlog
                {
                    BlogId = blog.Id,
                    FavoritedById = currentUserId,
                });
            }
            else
            {
                _dbContext.FavoriteBlogs.RemoveRange(blog.FavoriteBlogs);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

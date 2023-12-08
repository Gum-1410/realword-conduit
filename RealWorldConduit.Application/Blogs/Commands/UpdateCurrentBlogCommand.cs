using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealWorldConduit.Application.Blogs.DTOs;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public class UpdateCurrentBlogCommand : IRequest
    {
        // TODO : Implement Validation Later
        public string Title { get; set; }
        public UpdateRequestBlogDTO Body { get; set; }
    }

    internal class UpdateBlogCommandHandler : IRequestHandler<UpdateCurrentBlogCommand>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public UpdateBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task Handle(UpdateCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = await _dbContext.Blogs
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title) && x.AuthorId == _currentUser.Id, cancellationToken);

            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A {request.Title} blog is not found!");
            }

            blog.Title = request.Body.Title;
            blog.Description = request.Body.Description;
            blog.Content = request.Body.Content;

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

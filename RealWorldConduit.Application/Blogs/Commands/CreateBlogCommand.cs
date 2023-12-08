using MediatR;
using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Helpers;
using RealWorldConduit.Domain.Entities;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public class CreateBlogCommand : IRequest
    {
        // TODO : Implement Validation Later
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public List<string> TagList { get; set; }
    };

    internal class CreateBlogCommandHandler : IRequestHandler<CreateBlogCommand>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public CreateBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var currentUserID = (Guid)_currentUser.Id;

            var isBlogExisted = await _dbContext.Blogs
                                      .AsNoTracking()
                                      .AnyAsync(x => x.Title.Equals(request.Title) && x.AuthorId == _currentUser.Id, cancellationToken);

            if (isBlogExisted)
            {
                throw new RestException(HttpStatusCode.Conflict, $"A blog with {request.Title} title is existed!");
            }

            var newBlog = new Blog
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                AuthorId = currentUserID
            };

            // Combine the insertion of the new blog and tags using AddRange
            var requestFilteredTags = await RequestFilterTags(request, cancellationToken);
            newBlog.BlogTags = requestFilteredTags.Select(tag => new BlogTag { Blog = newBlog, Tag = tag }).ToList();

            // Add the new blog to the context
            _dbContext.Blogs.Add(newBlog);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<Tag>> RequestFilterTags(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var processedRequestTags = request.TagList.Distinct().ToList();

            var existingTags = await _dbContext.Tags
                                    .Where(x => processedRequestTags.Contains(x.Name))
                                    .ToListAsync(cancellationToken);

            var newTags = processedRequestTags
                         .Except(existingTags.Select(t => t.Name))
                         .Select(tag => new Tag { Name = tag })
                         .ToList();

            _dbContext.Tags.AddRange(newTags);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return existingTags.Concat(newTags).ToList();
        }
    }
}

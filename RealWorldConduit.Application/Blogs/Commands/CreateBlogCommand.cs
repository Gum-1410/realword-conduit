using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Common;
using RealWorldConduit.Application.Articles.DTOs;
using RealWorldConduit.Application.Users.DTOs;
using RealWorldConduit.Domain.Entities;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public class CreateBlogCommand : IRequestWithBaseResponse<BlogDTO>
    {
        // TODO : Implement Validation Later
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public List<string> TagList { get; set; }
    };

    internal class CreateBlogCommandHandler : IRequestWithBaseResponseHandler<CreateBlogCommand, BlogDTO>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public CreateBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task<BaseResponseDTO<BlogDTO>> Handle(CreateBlogCommand request, CancellationToken cancellationToken)
        {
            var requestFilteredTags = await RequestFilterTags(request, cancellationToken);

            var isBlogExisted = await _dbContext.Blogs.AnyAsync(x => x.Title.Equals(request.Title) && x.AuthorId == _currentUser.Id, cancellationToken);

            var author = await _dbContext.Users
                              .AsNoTracking()
                              .Include(x => x.FollowedUsers)
                              .FirstOrDefaultAsync(x => x.Id == _currentUser.Id, cancellationToken);

            if (isBlogExisted)
            {
                throw new RestException(HttpStatusCode.Conflict, $"A blog with {request.Title} title is existed!");
            }

            var newBlog = new Blog
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                AuthorId = (Guid)_currentUser.Id
            };

            _dbContext.Blogs.Add(newBlog);
            _dbContext.BlogsTag.AddRange(requestFilteredTags.Select(tag => new BlogTag { Blog = newBlog, Tag = tag }));

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully create new {request.Title} blog",
                Data = new BlogDTO
                {
                    Title = newBlog.Title,
                    Description = newBlog.Description,
                    Content = newBlog.Content,
                    TagList = newBlog.BlogTags.Select(x => x.Tag.Name).ToList(),
                    CreatedAt = newBlog.CreatedAt,
                    LastUpdatedAt = newBlog.LastUpdatedAt,
                    Profile = new ProfileDTO
                    {
                        Username = author.Username,
                        Email = author.Email,
                        Bio = author.Bio,
                        Following = author.FollowedUsers.Any(x => x.FollowerId == _currentUser.Id),
                        ProfileImage = author.ProfileImage
                    },
                    Favorited = false,
                    FavoritesCount = 0
                }
            };
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

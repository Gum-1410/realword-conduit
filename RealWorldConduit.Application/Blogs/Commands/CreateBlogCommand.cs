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
            var requestFilteredTags = await RequestTagsFilter(request, cancellationToken);
 
            var isBlogExisted = await _dbContext.Blogs
                                      .AsNoTracking()
                                      .AnyAsync(x => x.Title.Equals(request.Title), cancellationToken);

            if (isBlogExisted)
            {
                throw new RestException(HttpStatusCode.BadRequest, $"A blog with {request.Title} title is existed!");
            }

            var newBlog = new Blog
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                AuthorId = (Guid)_currentUser.Id
            };

            _dbContext.Blogs.Add(newBlog);

            // Add range to BlogTags based on filterd tags
            _dbContext.BlogsTag.AddRange(requestFilteredTags.Select(tag => new BlogTag { Blog = newBlog, Tag = tag }));

            await _dbContext.SaveChangesAsync(cancellationToken);

            BlogDTO blogDTO = await MapToBlogDTO(newBlog, cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully create new {newBlog.Title} blog",
                Data = blogDTO
            };
        }

        private async Task<BlogDTO> MapToBlogDTO(Blog blog, CancellationToken cancellationToken)
        {
            return await _dbContext.Blogs
                        .AsNoTracking()
                        .Select(x => new BlogDTO
                        {
                            Title = x.Title,
                            Description = x.Description,
                            Content = x.Content,
                            TagList = x.BlogTags.Select(x => x.Tag.Name).ToList(),
                            CreatedAt = x.CreatedAt,
                            LastUpdatedAt = x.LastUpdatedAt,
                            Profile = new ProfileDTO
                            {
                                Username = x.Author.Username,
                                Email = x.Author.Email,
                                Bio = x.Author.Bio,
                                Following = x.Author.FollowedUsers.Any(x => x.FollowerId == _currentUser.Id),
                                ProfileImage = x.Author.ProfileImage
                            },
                            Favorited = x.FavoriteBlogs.Any(x => x.FavoritedById == _currentUser.Id),
                            FavoritesCount = x.FavoriteBlogs.Count(x => x.BlogId == blog.Id)
                        })
                        .FirstOrDefaultAsync(x => x.Title.Equals(blog.Title), cancellationToken);
        }

        private async Task<List<Tag>> RequestTagsFilter(CreateBlogCommand request, CancellationToken cancellationToken)
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

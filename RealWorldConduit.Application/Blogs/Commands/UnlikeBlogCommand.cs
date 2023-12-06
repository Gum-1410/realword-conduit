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
    public record UnlikeBlogCommand(string Title) : IRequestWithBaseResponse<BlogDTO>;
    internal class UnlikeBlogCommandHandler : IRequestWithBaseResponseHandler<UnlikeBlogCommand, BlogDTO>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public UnlikeBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task<BaseResponseDTO<BlogDTO>> Handle(UnlikeBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = await _dbContext.Blogs
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title), cancellationToken);

            var favoritesBlog = await _dbContext.FavoriteBlogs
                                     .FirstOrDefaultAsync(x => x.BlogId == blog.Id && x.FavoritedById == _currentUser.Id);

            if (blog is null || favoritesBlog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Title} title is not found or you haven't like a blog with {request.Title} title yet!");
            }


            _dbContext.FavoriteBlogs.Remove(favoritesBlog);
            await _dbContext.SaveChangesAsync(cancellationToken);

            BlogDTO blogDTO = await MapToBlogDTO(blog, cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully unlike {request.Title} blog",
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
    }
}

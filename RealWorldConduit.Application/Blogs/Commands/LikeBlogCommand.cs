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
    // TODO : Implement Upsert Like A Blog Instead Of Seperate POST And DELETE
    public record LikeBlogCommand(string Title) : IRequestWithBaseResponse<BlogDTO>;
    internal class LikeBlogCommandHandler : IRequestWithBaseResponseHandler<LikeBlogCommand, BlogDTO>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public LikeBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }
        public async Task<BaseResponseDTO<BlogDTO>> Handle(LikeBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = await _dbContext.Blogs
                            .AsSplitQuery()
                            .Include(x => x.FavoriteBlogs)
                            .Include(x => x.Author)
                                .ThenInclude(y => y.FollowedUsers)
                            .Include(x => x.BlogTags)
                                .ThenInclude(y => y.Tag)
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title), cancellationToken);

            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Title} title is not found!");
            }

            if (blog.FavoriteBlogs.Any(x => x.FavoritedById == _currentUser.Id))
            {
                throw new RestException(HttpStatusCode.Conflict, $"You already like a blog with {request.Title} title!");
            }

            _dbContext.FavoriteBlogs.Add(new FavoriteBlog
            {
                BlogId = blog.Id,
                FavoritedById = (Guid)_currentUser.Id,
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully like {request.Title} blog",
                Data = new BlogDTO
                {
                    Title = blog.Title,
                    Description = blog.Description,
                    Content = blog.Content,
                    TagList = blog.BlogTags.Select(x => x.Tag.Name).ToList(),
                    CreatedAt = blog.CreatedAt,
                    LastUpdatedAt = blog.LastUpdatedAt,
                    Profile = new ProfileDTO
                    {
                        Username = blog.Author.Username,
                        Email = blog.Author.Email,
                        Bio = blog.Author.Bio,
                        Following = blog.Author.FollowedUsers.Any(x => x.FollowerId == _currentUser.Id),
                        ProfileImage = blog.Author.ProfileImage
                    },
                    Favorited = blog.FavoriteBlogs.Any(x => x.FavoritedById == _currentUser.Id),
                    FavoritesCount = blog.FavoriteBlogs.Count()
                }
            };
        }
    }
}

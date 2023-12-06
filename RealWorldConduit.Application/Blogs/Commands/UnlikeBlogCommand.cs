using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Common;
using RealWorldConduit.Application.Articles.DTOs;
using RealWorldConduit.Application.Users.DTOs;
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
                            .AsSplitQuery()
                            .Include(x => x.FavoriteBlogs)
                            .Include(x => x.Author)
                                .ThenInclude(y => y.FollowedUsers)
                            .Include(x => x.BlogTags)
                                .ThenInclude(y => y.Tag)
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title), cancellationToken);


            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Title} title is not found or you haven't like a blog with {request.Title} title yet!");
            }

            if (!blog.FavoriteBlogs.Any(x => x.FavoritedById == _currentUser.Id))
            {
                throw new RestException(HttpStatusCode.Conflict, $"You haven't like a blog with {request.Title} title yet!");
            }

            _dbContext.FavoriteBlogs.Remove(blog.FavoriteBlogs.FirstOrDefault(x => x.BlogId == blog.Id));
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully unlike {request.Title} blog",
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

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Common;
using RealWorldConduit.Application.Articles.DTOs;
using RealWorldConduit.Application.Blogs.DTOs;
using RealWorldConduit.Application.Users.DTOs;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;

namespace RealWorldConduit.Application.Blogs.Commands
{
    public class UpdateCurrentBlogCommand : IRequestWithBaseResponse<BlogDTO>
    {
        // TODO : Implement Validation Later
        public string Title { get; set; }
        public UpdateRequestBlogDTO Body { get; set; }
    }

    internal class UpdateBlogCommandHandler : IRequestWithBaseResponseHandler<UpdateCurrentBlogCommand, BlogDTO>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public UpdateBlogCommandHandler(MainDbContext dbContext, ICurrentUser currentUser, IHttpContextAccessor httpContext)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<BaseResponseDTO<BlogDTO>> Handle(UpdateCurrentBlogCommand request, CancellationToken cancellationToken)
        {
            var blog = await _dbContext.Blogs
                            .AsSplitQuery()
                            .Include(x => x.BlogTags)
                                .ThenInclude(y => y.Tag)
                            .Include(x => x.FavoriteBlogs)
                            .FirstOrDefaultAsync(x => x.Title.Equals(request.Title) && x.AuthorId == _currentUser.Id, cancellationToken);

            var author = await _dbContext.Users
                              .AsNoTracking()
                              .Include(x => x.FollowedUsers)
                              .FirstOrDefaultAsync(x => x.Id == _currentUser.Id, cancellationToken);

            if (blog is null)
            {
                throw new RestException(HttpStatusCode.NotFound, $"A blog with {request.Title} title is not found!");
            }

            blog.Title = request.Body.Title;
            blog.Description = request.Body.Description;
            blog.Content = request.Body.Content;

            _dbContext.Blogs.Update(blog);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<BlogDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully update {request.Title} blog",
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
                        Username = author.Username,
                        Email = author.Email,
                        Bio = author.Bio,
                        Following = author.FollowedUsers.Any(x => x.FollowerId == author.Id),
                        ProfileImage = author.ProfileImage
                    },
                    Favorited = blog.FavoriteBlogs.Any(x => x.FavoritedById == author.Id),
                    FavoritesCount = blog.FavoriteBlogs.Count()
                }
            };
        }
    }
}

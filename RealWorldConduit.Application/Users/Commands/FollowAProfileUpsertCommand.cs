using MediatR;
using Microsoft.EntityFrameworkCore;
using RealworldConduit.Infrastructure.Common;
using RealWorldConduit.Domain.Entities;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;


namespace RealWorldConduit.Application.Users.Commands
{
    // TODO : Implement Validation Later On
    public record FollowAProfileUpsertCommand(string Username) : IRequest;
    internal class FollowAProfileCommandHandler : IRequestHandler<FollowAProfileUpsertCommand>
    {
        private readonly MainDbContext _dbContext;
        private readonly ICurrentUser _currentUser;

        public FollowAProfileCommandHandler(MainDbContext dbContext, ICurrentUser currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task Handle(FollowAProfileUpsertCommand request, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.Users
                                .Include(x => x.FollowedUsers)
                                .FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken);

            if (profile is null)
            {
                throw new RestException(HttpStatusCode.NotFound, "User not found!");
            }

            if (!profile.FollowedUsers.Any(x => x.FollowerId == _currentUser.Id))
            {
                _dbContext.Followers.Add(new UserFollower
                {
                    FollowedUserId = profile.Id,
                    FollowerId = (Guid)_currentUser.Id
                });
            }
            else
            {
                _dbContext.Followers.RemoveRange(profile.FollowedUsers);

            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

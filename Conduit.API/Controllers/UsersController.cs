using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealWorldConduit.Application.Users.Commands;
using RealWorldConduit.Application.Users.Queries;

namespace Conduit.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("/api/user")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var currentUser = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return Ok(currentUser);
        }

        [Authorize]
        [HttpPut("/api/user")]
        public async Task<IActionResult> UpdateCurrentUser(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var updateCurrentUser = await _mediator.Send(request, cancellationToken);
            return Ok(updateCurrentUser);
        }

        [Authorize]
        [HttpPost("/api/user/refresh-token")]
        public async Task<IActionResult> RefreshToken(UserRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _mediator.Send(request, cancellationToken);
            return Ok(refreshToken);
        }

        [Authorize]
        [HttpDelete("/api/user/revoke-token")]
        public async Task<IActionResult> RevokeToken(UserRevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var revokeToken = await _mediator.Send(request, cancellationToken);
            return Ok(revokeToken);
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin(UserLoginCommand request, CancellationToken cancellationToken)
        {
            var existedUser = await _mediator.Send(request, cancellationToken);
            return Ok(existedUser);
        }

        [HttpPost("register")]
        public async Task<IActionResult> UserRegister(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var newUser = await _mediator.Send(request, cancellationToken);
            return Ok(newUser);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetAProfile([FromRoute] GetAProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _mediator.Send(request, cancellationToken);
            return Ok(profile);
        }

        [Authorize]
        [HttpPost("{username}/follow")]
        public async Task<IActionResult> FollowAProfile([FromRoute] FollowAProfileUpsertCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(request, cancellationToken);
                var profile = await _mediator.Send(new GetAProfileQuery(request.Username), cancellationToken);
                return Ok(profile);
            }
            catch(Exception ex) 
            {
                throw;
            }
        }      
    }
}

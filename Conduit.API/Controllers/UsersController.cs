﻿using MediatR;
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
        public async Task<IActionResult> GetCurrentUser()
        {
            var currentUser = await _mediator.Send(new GetCurrentUserQuery());
            return Ok(currentUser);
        }

        [Authorize]
        [HttpPut("/api/user")]
        public async Task<IActionResult> UpdateCurrentUser()
        {
            throw new NotImplementedException();
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
    }
}
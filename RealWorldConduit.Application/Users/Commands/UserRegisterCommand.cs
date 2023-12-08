using FluentValidation;
using RealworldConduit.Infrastructure.Common;
using RealworldConduit.Infrastructure.Helpers;
using RealWorldConduit.Domain.Entities;
using RealWorldConduit.Infrastructure;
using RealWorldConduit.Infrastructure.Auth;
using RealWorldConduit.Infrastructure.Common;
using System.Net;


namespace RealWorldConduit.Application.Users.Commands
{
    public class UserRegisterCommand : IRequestWithBaseResponse<AuthDTO>
    {
        public string Username { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public class UserRegisterCommandValidator : AbstractValidator<UserRegisterCommand>
    {
        private readonly MainDbContext _dbContext;
        public UserRegisterCommandValidator(MainDbContext dbContext)
        {
            _dbContext = dbContext;
         
            RuleFor(x => x.Username).NotEmpty()
                                    .OverridePropertyName("username")
                                    .WithMessage("Invalid username");

            RuleFor(x => x.Username).Must(username =>
                                {
                                    var isExisted = _dbContext.Users.Any(u => u.Username == username);
                                    return !isExisted;
                                }).OverridePropertyName("username")
                                  .WithMessage("Username already existed");

            RuleFor(x => x.Email).NotEmpty()
                                 .EmailAddress()
                                 .OverridePropertyName("email")
                                 .WithMessage("Invalid email");

            RuleFor(x => x.Email).Must(mail =>
                                 {
                                     var isExisted = _dbContext.Users.Any(u => u.Email == mail);
                                     return !isExisted;
                                 }).OverridePropertyName("email")
                                   .WithMessage("Email already existed");

            RuleFor(x => x.Password).NotEmpty()
                                    .MinimumLength(6)
                                    .OverridePropertyName("password")
                                    .WithMessage("Invalid password or password must be more than 6 characters");
        }
    }

    internal class UserRegisterCommandHandler : IRequestWithBaseResponseHandler<UserRegisterCommand, AuthDTO>
    {
        private readonly MainDbContext _dbContext;
        private readonly IAuthService _authService;

        public UserRegisterCommandHandler(MainDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        public async Task<BaseResponseDTO<AuthDTO>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
        {
            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = _authService.HashPassword(request.Password)
            };
            _dbContext.Users.Add(newUser);

            var refreshToken = _authService.GenerateRefreshToken(newUser);
            _dbContext.RefreshTokens.Add(refreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BaseResponseDTO<AuthDTO>
            {
                Code = HttpStatusCode.OK,
                Message = $"Successfully register user {request.Email}",
                Data = new AuthDTO
                {
                    AccessToken = _authService.GenerateToken(newUser),
                    RefreshToken = refreshToken.AccessToken,
                }
            };
        }
    }
}

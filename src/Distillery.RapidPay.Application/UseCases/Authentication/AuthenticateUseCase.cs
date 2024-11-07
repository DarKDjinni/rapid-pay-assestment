namespace Distillery.RapidPay.Application.UseCases.Authentication;

using Microsoft.AspNetCore.Identity;

public static class AuthenticationUseCase
{
    public static AuthenticationRequest CreateRequest(string username, string password) => new()
    {
        Username = username,
        Password = password
    };

    public sealed record AuthenticationRequest : IRequest<Result<AuthenticationResponse>>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public sealed record AuthenticationResponse
    {
        public required string Username { get; set; }
        public required string Token { get; set; }
    }

    public sealed class AuthenticationValidator : AbstractValidator<AuthenticationRequest>
    {
        public AuthenticationValidator()
        {
            this.RuleFor(x => x.Username)
                .NotEmpty();

            this.RuleFor(x => x.Password)
                .NotEmpty();
        }
    }

    public sealed class AuthenticationHandler(UserManager<IdentityUser> userManager, IJsonWebTokenService jsonWebTokenService) : IRequestHandler<AuthenticationRequest, Result<AuthenticationResponse>>
    {
        public async Task<Result<AuthenticationResponse>> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                return Result.Unauthorized("Invalid username or password.");
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result.Unauthorized("Invalid username or password.");
            }

            string token = await jsonWebTokenService.GenerateTokenAsync(user);

            return Result.Success(new AuthenticationResponse { Token = token, Username = user.NormalizedUserName! });
        }
    }
}

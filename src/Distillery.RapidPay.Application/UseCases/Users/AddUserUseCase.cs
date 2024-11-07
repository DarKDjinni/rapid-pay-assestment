namespace Distillery.RapidPay.Application.UseCases.Users;

using Microsoft.AspNetCore.Identity;

public static class AddUserUseCase
{
    public static AddUserRequest CreateRequest(string email, string username, string password) => new()
    {
        Email = email,
        Username = username,
        Password = password
    };

    public sealed record AddUserRequest : IRequest<Result<AddUserResponse>>
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public sealed record AddUserResponse
    {
        public required string Username { get; set; }
        public required string Token { get; set; }
    }

    public sealed class AddUserValidator : AbstractValidator<AddUserRequest>
    {
        public AddUserValidator()
        {
            this.RuleFor(x => x.Email)
                .EmailAddress()
                .NotEmpty();

            this.RuleFor(x => x.Username)
                .NotEmpty();

            this.RuleFor(x => x.Password)
                .NotEmpty();
        }
    }

    public sealed class AddUserHandler(UserManager<IdentityUser> userManager, IJsonWebTokenService jsonWebTokenService) : IRequestHandler<AddUserRequest, Result<AddUserResponse>>
    {
        public async Task<Result<AddUserResponse>> Handle(AddUserRequest request, CancellationToken cancellationToken)
        {
            var user = new IdentityUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                string token = await jsonWebTokenService.GenerateTokenAsync(user);

                return Result.Success(new AddUserResponse { Token = token, Username = user.NormalizedUserName! });
            }

            var validationErrors = result.Errors.Select(x => new ValidationError
            {
                ErrorCode = x.Code,
                ErrorMessage = x.Description,
                Identifier = x.Code,
                Severity = ValidationSeverity.Error
            });

            return Result.Invalid(validationErrors);
        }
    }
}

namespace Distillery.RapidPay.Application.UseCases.CreditCards;

public static class GetCreditCardBalanceUseCase
{
    public static GetCreditCardBalanceRequest CreateRequest(string cardNumber) => new()
    {
        CardNumber = cardNumber
    };

    public sealed record GetCreditCardBalanceRequest : IRequest<Result<GetCreditCardBalanceResponse>>
    {
        public required string CardNumber { get; set; }
    }

    public sealed record GetCreditCardBalanceResponse
    {
        public string? CardNumber { get; set; }
        public decimal? Balance { get; set; }

        internal sealed class Mapping : Profile
        {
            public Mapping() => this.CreateMap<CreditCard, GetCreditCardBalanceResponse>();
        }
    }

    public sealed class GetCreditCardBalanceValidator : AbstractValidator<GetCreditCardBalanceRequest>
    {
        public GetCreditCardBalanceValidator() => _ = this.RuleFor(x => x.CardNumber).IsCreditCardNumber();
    }

    public sealed class GetCreditCardBalanceHandler(IEncryptionService encryptionService, IApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<GetCreditCardBalanceRequest, Result<GetCreditCardBalanceResponse>>
    {
        public async Task<Result<GetCreditCardBalanceResponse>> Handle(GetCreditCardBalanceRequest request, CancellationToken cancellationToken)
        {
            string encryptedCardNumber = await encryptionService.EncryptAsync(request.CardNumber);

            var response = await dbContext.CreditCards
                .Where(x => x.CardNumber == encryptedCardNumber)
                .Where(x => x.UserId == currentUserService.UserId)
                .ProjectTo<GetCreditCardBalanceResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (response == null)
            {
                return Result.NotFound($"The requested credit card information is not available for user: {currentUserService.UserName}.");
            }
            response.CardNumber = await encryptionService.DecryptAsync(response.CardNumber!);

            return Result.Success(response);
        }
    }
}

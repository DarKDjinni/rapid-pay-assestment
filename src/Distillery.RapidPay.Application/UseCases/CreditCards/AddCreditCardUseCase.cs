namespace Distillery.RapidPay.Application.UseCases.CreditCards;

public static class AddCreditCardUseCase
{
    public static AddCreditCardRequest CreateRequest(string cardNumber, string ownerName, string ccv, decimal creditLine, byte month, ushort year) => new()
    {
        CardNumber = cardNumber,
        OwnerName = ownerName,
        Ccv = ccv,
        CreditLine = creditLine,
        Month = month,
        Year = year
    };

    public sealed record AddCreditCardRequest : IRequest<Result<CreditCardResponse>>
    {
        public required string CardNumber { get; set; }
        public required string OwnerName { get; set; }
        public required string Ccv { get; set; }
        public required byte Month { get; set; }
        public required ushort Year { get; set; }
        public required decimal CreditLine { get; set; }
    }

    public sealed record CreditCardResponse
    {
        public string? CardNumber { get; set; }
        public decimal? CreditLine { get; set; }
        public decimal? CreditSpent { get; set; }

        internal sealed class Mapping : Profile
        {
            public Mapping() => this.CreateMap<CreditCard, CreditCardResponse>();
        }
    }

    public sealed class AddCreditCardValidator : AbstractValidator<AddCreditCardRequest>
    {
        public AddCreditCardValidator(TimeProvider timeProvider)
        {
            _ = this.RuleFor(x => x.CardNumber)
                .IsCreditCardNumber();

            _ = this.RuleFor(x => x.Ccv)
                .IsCcv();

            _ = this.RuleFor(x => x.CreditLine)
                .NotEmpty()
                .GreaterThan(0);

            _ = this.RuleFor(x => x.Month)
                .InclusiveBetween((byte)1, (byte)12);

            _ = this.RuleFor(x => x.Month)
                .GreaterThanOrEqualTo((byte)timeProvider.GetUtcNow().Month)
                .When(x => x.Year == (ushort)timeProvider.GetUtcNow().Year);

            _ = this.RuleFor(x => x.OwnerName)
                .NotEmpty();

            _ = this.RuleFor(x => x.Year)
                .NotEmpty()
                .GreaterThanOrEqualTo((ushort)timeProvider.GetUtcNow().Year);
        }
    }

    public sealed class AddCreditCardHandler(IEncryptionService encryptionService, IApplicationDbContext dbContext, IMapper mapper) : IRequestHandler<AddCreditCardRequest, Result<CreditCardResponse>>
    {
        public async Task<Result<CreditCardResponse>> Handle(AddCreditCardRequest request, CancellationToken cancellationToken)
        {
            string encryptedCardNumber = await encryptionService.EncryptAsync(request.CardNumber);
            string encryptedCcv = await encryptionService.EncryptAsync(request.Ccv);

            var creditCard = new CreditCard
            {
                CardNumber = encryptedCardNumber,
                Ccv = encryptedCcv,
                CreditLine = request.CreditLine,
                Month = request.Month,
                OwnerName = request.OwnerName,
                Year = request.Year
            };

            _ = dbContext.CreditCards.Add(creditCard);
            _ = await dbContext.SaveChangesAsync(cancellationToken);

            var response = mapper.Map<CreditCardResponse>(creditCard);
            response.CardNumber = $"***********{request.CardNumber[^4..]}";

            return Result.Created(response, string.Empty);
        }
    }
}

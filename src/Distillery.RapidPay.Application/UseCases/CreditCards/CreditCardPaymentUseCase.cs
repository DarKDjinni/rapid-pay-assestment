namespace Distillery.RapidPay.Application.UseCases.CreditCards;

public static class CreditCardPaymentUseCase
{
    public static CreditCardPaymentRequest CreateRequest(string cardNumber, decimal amount) => new()
    {
        CardNumber = cardNumber,
        Amount = amount
    };

    public sealed record CreditCardPaymentRequest : IRequest<Result>
    {
        public required string CardNumber { get; set; }
        public required decimal Amount { get; set; }
    }

    public sealed class CreditCardPaymentValidator : AbstractValidator<CreditCardPaymentRequest>
    {
        public CreditCardPaymentValidator()
        {
            _ = this.RuleFor(x => x.Amount)
                .NotEmpty()
                .GreaterThan(0.01m);

            _ = this.RuleFor(x => x.CardNumber)
                .IsCreditCardNumber();
        }
    }

    public sealed class CreditCardPaymentHandler(IEncryptionService encryptionService, IApplicationDbContext dbContext, ICurrentUserService currentUserService, IUniversalFeesExchange universalFeesExchange) : IRequestHandler<CreditCardPaymentRequest, Result>
    {
        public async Task<Result> Handle(CreditCardPaymentRequest request, CancellationToken cancellationToken)
        {
            string cardNumber = await encryptionService.EncryptAsync(request.CardNumber);
            var creditCard = await dbContext.CreditCards
                .Where(x => x.CardNumber == cardNumber)
                .Where(x => x.UserId == currentUserService.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            if (creditCard == null)
            {
                return Result.NotFound();
            }

            creditCard.CreditLine -= request.Amount + await universalFeesExchange.GetFeeAsync();

            if (creditCard.CreditLine < 0)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = "InsufficientFunds",
                    ErrorMessage = "Insufficient funds to complete the operation.",
                    Severity = ValidationSeverity.Error
                });
            }

            try
            {
                _ = dbContext.CreditCards.Update(creditCard);
                _ = await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Invalid(new ValidationError
                {
                    Identifier = "DuplicatedOperation",
                    ErrorMessage = "The operation couldn't be completed, please try again.",
                    Severity = ValidationSeverity.Warning
                });
            }

            return Result.SuccessWithMessage("Operation completed successfully.");
        }
    }
}

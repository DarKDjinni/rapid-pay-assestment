namespace Distillery.RapidPay.Application.Common.Validators;

public static class CreditCardValidators
{
    public static IRuleBuilderOptions<T, string> IsCreditCardNumber<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder
            .NotEmpty()
            .Matches("^\\d{15}$")
                .WithMessage("Credit Card numbers must be 15 digits long.");

    public static IRuleBuilderOptions<T, string> IsCcv<T>(this IRuleBuilder<T, string> ruleBuilder) => ruleBuilder
            .NotEmpty()
            .Matches(@"^\d{3,4}$")
                .WithMessage("CCV must be 3-4 digits long.");
}

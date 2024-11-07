namespace Distillery.RapidPay.CardManagementApi.Controllers;

using static Distillery.RapidPay.Application.UseCases.CreditCards.AddCreditCardUseCase;
using static Distillery.RapidPay.Application.UseCases.CreditCards.CreditCardPaymentUseCase;
using static Distillery.RapidPay.Application.UseCases.CreditCards.GetCreditCardBalanceUseCase;

/// <summary>
/// The CreditCardController is responsible for all interactions with the CreditCard resource
/// </summary>
/// <param name="mediator"></param>
[Authorize]
[ApiController]
[Route("credit-card")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class CreditCardController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Used to create a new CreditCard resource.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="201">Returns a summary of the CreditCard information.</response>
    /// <response code="400">If a validation error occurred while processing the request.</response>
    /// <response code="401">If the user couldn't be authenticated.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    [HttpPost]
    [ProducesResponseType<CreditCardResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddCreditCardAsync([FromBody] AddCreditCardRequest request)
    {
        var response = await mediator.Send(request);

        return response.ToApiResult();
    }

    /// <summary>
    /// Performs a payment operation using the provided credit card.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returned when the payment was successful</response>
    /// <response code="400">If a validation error occurred while processing the request.</response>
    /// <response code="401">If the user couldn't be authenticated.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    [HttpPost]
    [Route("payment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessPaymentAsync([FromBody] CreditCardPaymentRequest request)
    {
        var response = await mediator.Send(request);

        return response.ToApiResult();
    }

    /// <summary>
    /// Gets the balance associated to the input Credit Card
    /// </summary>
    /// <param name="cardNumber">The CardNumber used to get the balance.</param>
    /// <returns></returns>
    /// <response code="200">Returns the CreditCard number and balance.</response>
    /// <response code="400">If a validation error occurred while processing the request.</response>
    /// <response code="401">If the user couldn't be authenticated.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    [HttpGet]
    [Route("{cardNumber}/balance")]
    [ProducesResponseType<GetCreditCardBalanceResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBalanceAsync([FromRoute] string cardNumber)
    {
        var response = await mediator.Send(CreateRequest(cardNumber));

        return response.ToApiResult();
    }
}

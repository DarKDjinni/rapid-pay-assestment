namespace Distillery.RapidPay.CardManagementApi.Controllers;

using static Distillery.RapidPay.Application.UseCases.Authentication.AuthenticationUseCase;

/// <summary>
/// The AuthenticationController is used to Authenticate and Authorize users
/// </summary>
/// <param name="mediator"></param>
[Authorize]
[ApiController]
[Route("auth")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Used to generate an Auth Token (JWT).
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returns the Username and Token.</response>
    /// <response code="400">If a validation error occurred while processing the request.</response>
    /// <response code="401">If the user couldn't be authenticated.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    [HttpPost]
    [Route("token")]
    [AllowAnonymous]
    [ProducesResponseType<AuthenticationResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthenticationRequest request)
    {
        var response = await mediator.Send(request);

        return response.ToApiResult();
    }
}

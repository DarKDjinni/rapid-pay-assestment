namespace Distillery.RapidPay.CardManagementApi.Controllers;

using static Distillery.RapidPay.Application.UseCases.Users.AddUserUseCase;

/// <summary>
/// The AuthenticationController is used to Authenticate and Authorize users
/// </summary>
/// <param name="mediator"></param>
[Authorize]
[ApiController]
[Route("user")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public class UserController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Used to register a new User.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returns the Username and Token.</response>
    /// <response code="400">If a validation error occurred while processing the request.</response>
    /// <response code="500">If an error occurred while processing the request.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType<AddUserResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddUserAsync([FromBody] AddUserRequest request)
    {
        var response = await mediator.Send(request);

        return response.ToApiResult();
    }
}

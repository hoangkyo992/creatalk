using Auth.Application.Features.Auth;

namespace Auth.Api.Controllers;

[Authorize]
[ApiController]
[Route($"{ApiConstants.ApiPrefix}/auth")]
[EnableRateLimiting(nameof(RateLimitPolicy.IdentityFixed))]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Sign in
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("signin")]
    [ProducesResponseType(typeof(ApiResult<SignIn.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Signin([FromBody] SignIn.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Signout
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("signout")]
    [ProducesResponseType(typeof(ApiResult<SignOut.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Signout([FromBody] SignOut.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("force-signout")]
    [ProducesResponseType(typeof(ApiResult<Revoke.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Revoke([FromQuery] Revoke.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Change current user's password
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResult<ChangePassword.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePassword.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("me")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApplicationIdentity), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> GetIdentity(CancellationToken cancellationToken = default)
    {
        Request.Headers.TryGetValue(DefaultRequestHeaders.Authorization, out StringValues tokens);
        if (string.IsNullOrWhiteSpace(tokens) || !tokens.ToString().StartsWith("Bearer"))
            return Unauthorized(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);

        var token = tokens.ToString().Split(" ")[1].Trim();
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);

        var result = await mediator.Send(new GetIdentity.Request(token), cancellationToken);
        if (result.IsSuccess)
            return Ok(result.Result.AuthData);
        return Unauthorized(CommonErrorMessages.UNAUTHENTICATED_EXCEPTION);
    }

    [HttpGet("accesses"), MapToApiVersion(ApiConstants.Ver1_0)]
    [ProducesResponseType(typeof(ApiResult<GetAccesses.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAccessesAsync([FromQuery] GetAccesses.Request request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("check-access")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> CheckAccess([FromBody] CheckAccess.Request request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess && result.Result.IsAccessable ? Ok(true) : (IActionResult)Ok(false);
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResult<Renew.Result>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Renew([FromBody] Renew.Command command, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
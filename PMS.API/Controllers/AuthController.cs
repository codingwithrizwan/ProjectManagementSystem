using PMS.Application.UseCases.Commands.Auth;

namespace PMS.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController(IMediator mediator) : ControllerBase // I used primary constructor 
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await mediator.Send(new RegisterCommand(dto));
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await mediator.Send(new LoginCommand(dto));
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            if (string.Equals(result.Message, "Invalid credentials", StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized(result);
            }

            return BadRequest(result);
        }
    }
}

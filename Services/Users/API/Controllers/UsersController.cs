using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Users.API.DTOs;
using Users.Application.Commands;
using Users.Application.DTOs;
using Users.Domain;
using Users.Infrastructure.Configuration;

namespace Users.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly JwtConfigOptions jwtConfigOptions;
        public UsersController
        (
            IMediator mediator,
            IOptions<JwtConfigOptions> options
        )
        {
            this.mediator = mediator;
            jwtConfigOptions = options.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(RegisterUserCommand command)
        {
            await mediator.Send(command);
            return Created();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginUserDTO data)
        {
            var command = new LoginUserCommand(data);
            var response = await mediator.Send(command);
            if (response.token != null)
            {
                var cookiesOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(jwtConfigOptions.ExpiresMinutes)
                };
                Response.Cookies.Append("access_token", response.token, cookiesOptions);
                return Accepted();
            }
            return Unauthorized(response.message);
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var command = new GetUserByIdCommand(userId);
            var user = await mediator.Send(command);
            return Ok(user);
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserRole? role = null)
        {
            var users = await mediator.Send(role != null ? new GetUsersByRoleCommand((UserRole)role) : new GetUsersCommand());
            return Ok(users);
        }
        [HttpGet("name/{username}")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var command = new GetUserByUserNameCommand(username);
            var user = await mediator.Send(command);
            return Ok(user);
        }
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var command = new GetUserByEmailCommand(email);
            var user = await mediator.Send(command);
            return Ok(user);
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new GetUserByIdCommand(Guid.Parse(userId!));
            var user = await mediator.Send(command);
            return Ok(user);
        }
        [Authorize]
        [HttpPut("me/change_password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO data)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new ChangePasswordCommand(Guid.Parse(userId!),data.Password);
            await mediator.Send(command);
            return Accepted();
        }
        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");
            return Accepted();
        }
        [Authorize]
        [HttpDelete("me")]
        public async Task<IActionResult> UnRegister()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new UnRegisterUserCommand(Guid.Parse(userId!));
            await mediator.Send(command);
            return Accepted();
        }
    }
}

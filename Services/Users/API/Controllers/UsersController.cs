using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commands;
using Users.Domain;

namespace Users.API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator mediator;
        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(RegisterUserCommand command)
        {
            await mediator.Send(command);
            return Created();
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
    }
}

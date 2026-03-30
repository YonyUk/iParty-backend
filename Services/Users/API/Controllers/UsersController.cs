using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Commands;
using Users.Application.DTOs;

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
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var command = new GetUserByIdCommand(userId);
            var user = await mediator.Send(command);
            return Ok(user);
        }
    }
}

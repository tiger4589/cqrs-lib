using CQRSLib.Core;
using DemoCqrsApplication;
using Microsoft.AspNetCore.Mvc;

namespace DemoCqrs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IDispatcher dispatcher) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromBody] GetUserQuery userQuery)
        {
            var user = await dispatcher.DispatchAsync<GetUserQuery, GetUserQueryResult>(userQuery);
            return Ok(user);
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get([FromBody] GetUsersQuery userQuery)
        {
            var users = await dispatcher.DispatchAsync<GetUsersQuery, GetUsersQueryResult>(userQuery);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserCommand command)
        {
            await dispatcher.DispatchAsync(command);
            return Ok();
        }

        [HttpDelete()]
        public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
        {
            await dispatcher.DispatchAsync(command);
            return Ok();
        }
    }
}

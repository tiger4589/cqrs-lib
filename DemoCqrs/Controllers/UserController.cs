using CQRSLib.Core;
using DemoCqrsApplication.AddUser;
using DemoCqrsApplication.DeleteUser;
using DemoCqrsApplication.GetUser;
using Microsoft.AspNetCore.Mvc;

namespace DemoCqrs.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        GetUserQuery userQuery = new(id);
        var user = await dispatcher.DispatchAsync<GetUserQuery, GetUserQueryResult>(userQuery);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddUserCommand command)
    {
        await dispatcher.DispatchAsync(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
    {
        await dispatcher.DispatchAsync(command);
        return Ok();
    }
}
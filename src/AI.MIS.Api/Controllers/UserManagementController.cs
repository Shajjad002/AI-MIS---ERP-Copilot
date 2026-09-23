using AI.MIS.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Administrator")]
public sealed class UserManagementController(IUserManagementService users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken) => Ok(await users.ListAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        try { return Ok(await users.CreateAsync(command, cancellationToken)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return Conflict(new { message = exception.Message }); }
    }

    [HttpPut("{id:guid}/access")]
    public async Task<IActionResult> UpdateAccess(Guid id, [FromBody] UpdateUserAccessCommand command, CancellationToken cancellationToken)
    {
        try { await users.UpdateAccessAsync(id, command, cancellationToken); return NoContent(); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
    }
}

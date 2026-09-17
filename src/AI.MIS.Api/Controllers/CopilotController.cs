using AI.MIS.Application.Copilot;
using Microsoft.AspNetCore.Mvc;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/copilot")]
public sealed class CopilotController(ICopilotPlanningService planningService) : ControllerBase
{
    [HttpPost("interpret")]
    public async Task<IActionResult> Interpret([FromBody] InterpretRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await planningService.InterpretAsync(request.Question, cancellationToken)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = exception.Message }); }
    }

    public sealed record InterpretRequest(string Question);
}

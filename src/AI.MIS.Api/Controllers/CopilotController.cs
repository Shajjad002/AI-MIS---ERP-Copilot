using AI.MIS.Application.Copilot;
using AI.MIS.Application.Copilot.Models;
using Microsoft.AspNetCore.Mvc;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/copilot")]
public sealed class CopilotController(
    ICopilotPlanningService planningService,
    ISchemaMetadataProvider schemaMetadataProvider,
    ISqlQueryValidator sqlQueryValidator,
    IReadOnlyQueryExecutor queryExecutor) : ControllerBase
{
    [HttpPost("interpret")]
    public async Task<IActionResult> Interpret([FromBody] InterpretRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(await planningService.InterpretAsync(request.Question, cancellationToken)); }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = exception.Message }); }
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] InterpretRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var interpretation = await planningService.InterpretAsync(request.Question, cancellationToken);
            if (interpretation.NeedsClarification || string.IsNullOrWhiteSpace(interpretation.ProposedSql))
                return Ok(new { interpretation, result = (CopilotQueryResult?)null });

            var schema = await schemaMetadataProvider.GetApprovedSchemaAsync(cancellationToken);
            sqlQueryValidator.Validate(interpretation.ProposedSql, schema);
            var result = await queryExecutor.ExecuteAsync(interpretation.ProposedSql, cancellationToken);
            return Ok(new { interpretation, result });
        }
        catch (ArgumentException exception) { return BadRequest(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return StatusCode(StatusCodes.Status422UnprocessableEntity, new { message = exception.Message }); }
    }

    public sealed record InterpretRequest(string Question);
}

using AI.MIS.Application.Rag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize(Roles = "Administrator,MIS Analyst")]
public sealed class DocumentsController(IDocumentStore documentStore) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken cancellationToken)
        => Ok(await documentStore.ListAsync(cancellationToken));

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null)
            return BadRequest(new { error = "A document file is required." });

        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var userId))
            return Unauthorized();

        try
        {
            await using var stream = file.OpenReadStream();
            var document = await documentStore.SaveAsync(stream, file.FileName, file.ContentType, file.Length, userId, cancellationToken);
            return CreatedAtAction(nameof(List), new { id = document.Id }, document);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }
}

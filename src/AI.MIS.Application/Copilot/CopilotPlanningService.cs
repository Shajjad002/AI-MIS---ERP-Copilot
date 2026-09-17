using System.Text.Json;
using AI.MIS.Application.Copilot.Models;

namespace AI.MIS.Application.Copilot;

public sealed class CopilotPlanningService(ILlmClient llmClient, ISchemaMetadataProvider schemaMetadataProvider) : ICopilotPlanningService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<CopilotInterpretation> InterpretAsync(string question, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(question)) throw new ArgumentException("A question is required.", nameof(question));

        var schema = await schemaMetadataProvider.GetApprovedSchemaAsync(cancellationToken);
        var schemaJson = JsonSerializer.Serialize(schema, JsonOptions);
        var prompt = $$"""
            You are an ERP MIS assistant. Return only JSON matching the requested schema.
            Rules: interpret only from the approved schema below; use SELECT queries only; never use data-modifying SQL; never invent tables or columns; request clarification when needed.
            Approved schema: {{schemaJson}}
            """;
        var json = await llmClient.CompleteJsonAsync(prompt, question, cancellationToken);
        var interpretation = JsonSerializer.Deserialize<CopilotInterpretation>(json, JsonOptions)
            ?? throw new InvalidOperationException("The AI returned an empty structured response.");

        return interpretation with { ProposedSql = interpretation.ProposedSql?.Trim() };
    }
}

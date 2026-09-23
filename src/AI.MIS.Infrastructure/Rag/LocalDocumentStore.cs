using System.Text.Json;
using AI.MIS.Application.Rag;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace AI.MIS.Infrastructure.Rag;

public sealed class RagOptions
{
    public const string SectionName = "Rag";
    public string StoragePath { get; init; } = "App_Data/documents";
    public long MaxUploadBytes { get; init; } = 10 * 1024 * 1024;
}

public sealed class LocalDocumentStore(IOptions<RagOptions> options, IWebHostEnvironment environment) : IDocumentStore
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim gate = new(1, 1);

    public async Task<StoredDocument> SaveAsync(Stream content, string fileName, string contentType, long length, Guid uploadedBy, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (length <= 0 || length > settings.MaxUploadBytes)
            throw new InvalidOperationException($"Document size must be between 1 byte and {settings.MaxUploadBytes} bytes.");

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension is not ".txt" and not ".md" and not ".pdf" and not ".docx")
            throw new InvalidOperationException("Only .txt, .md, .pdf, and .docx documents are supported.");

        var root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, settings.StoragePath));
        Directory.CreateDirectory(root);
        var id = Guid.NewGuid();
        var storedName = $"{id:N}{extension}";
        await using (var target = File.Create(Path.Combine(root, storedName)))
            await content.CopyToAsync(target, cancellationToken);

        var document = new StoredDocument(id, Path.GetFileName(fileName), string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType, length, uploadedBy, DateTimeOffset.UtcNow);
        await gate.WaitAsync(cancellationToken);
        try
        {
            var metadataPath = Path.Combine(root, "index.json");
            var documents = File.Exists(metadataPath)
                ? JsonSerializer.Deserialize<List<StoredDocument>>(await File.ReadAllTextAsync(metadataPath, cancellationToken), JsonOptions) ?? []
                : [];
            documents.Add(document);
            await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(documents, JsonOptions), cancellationToken);
        }
        finally
        {
            gate.Release();
        }

        return document;
    }

    public async Task<IReadOnlyList<StoredDocument>> ListAsync(CancellationToken cancellationToken = default)
    {
        var root = Path.GetFullPath(Path.Combine(environment.ContentRootPath, options.Value.StoragePath));
        var metadataPath = Path.Combine(root, "index.json");
        if (!File.Exists(metadataPath)) return [];
        await gate.WaitAsync(cancellationToken);
        try
        {
            return JsonSerializer.Deserialize<List<StoredDocument>>(await File.ReadAllTextAsync(metadataPath, cancellationToken), JsonOptions) ?? [];
        }
        finally
        {
            gate.Release();
        }
    }
}

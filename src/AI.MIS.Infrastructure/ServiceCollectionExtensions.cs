using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AI.MIS.Application.Copilot;
using AI.MIS.Infrastructure.AI;
using AI.MIS.Infrastructure.Database;

namespace AI.MIS.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LlmOptions>(configuration.GetSection(LlmOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.AddHttpClient<ILlmClient, OpenAiCompatibleLlmClient>(client => client.Timeout = TimeSpan.FromSeconds(30));
        services.AddSingleton<ISchemaMetadataProvider, InMemorySchemaMetadataProvider>();
        services.AddSingleton<ISqlQueryValidator, SqlQueryValidator>();
        services.AddScoped<IReadOnlyQueryExecutor, SqlReadOnlyQueryExecutor>();
        return services;
    }
}

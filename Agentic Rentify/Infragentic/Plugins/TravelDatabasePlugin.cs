using System.ComponentModel;
using System.Text.Json;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agentic_Rentify.Infragentic.Plugins;
public class TravelDatabasePlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("semantic_travel_search")]
    [Description("Searches for general Egyptian travel tips, history, and hidden gems using Vector Search.")]
    public async Task<string> SearchAsync([Description("User's natural language query")] string query)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var vectorService = scope.ServiceProvider.GetRequiredService<IVectorDbService>(); // [cite: 238]
        var results = await vectorService.SearchByTextAsync("rentify_memory", query, 5); // [cite: 239, 240]
        return JsonSerializer.Serialize(results);
    }
}
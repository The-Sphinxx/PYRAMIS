using System.ComponentModel;
using System.Text.Json;
using Agentic_Rentify.Application.Features.Attractions.Queries.GetAllAttractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace Agentic_Rentify.Infragentic.Plugins;
public class DiscoveryPlugin(IServiceScopeFactory serviceScopeFactory)
{
    [KernelFunction("get_site_meta_data")]
    [Description("Gets all supported cities and categories in the system.")]
    public async Task<string> GetMetaDataAsync()
    {
        using var scope = serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetAllAttractionsQuery { PageSize = 50 }); // مثال [cite: 348, 349]
        return JsonSerializer.Serialize(result);
    }
}
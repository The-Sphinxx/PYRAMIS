using Agentic_Rentify.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agentic_Rentify.Application.Features.SyncVector;

public class EntitySavedToVectorDbEventHandler(IVectorDbService vectorDbService, ILogger<EntitySavedToVectorDbEventHandler> logger)
    : INotificationHandler<EntitySavedToVectorDbEvent>
{
    public async Task Handle(EntitySavedToVectorDbEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            const string collection = "rentify_memory";
            await vectorDbService.CreateCollectionIfNotExists(collection);
            await vectorDbService.SaveTextVector(
                collection,
                notification.EntityId.ToString(),
                notification.EntityType,
                notification.Text,
                notification.Name,
                notification.Price,
                notification.City);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the operation
            logger.LogWarning(ex, "Failed to sync entity {EntityId} of type {EntityType} to vector database. Operation will continue.", 
                notification.EntityId, notification.EntityType);
        }
    }
}

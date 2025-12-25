using Agentic_Rentify.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agentic_Rentify.Application.Features.SyncVector;

public class EntityDeletedFromVectorDbEventHandler(IVectorDbService vectorDbService, ILogger<EntityDeletedFromVectorDbEventHandler> logger)
    : INotificationHandler<EntityDeletedFromVectorDbEvent>
{
    public async Task Handle(EntityDeletedFromVectorDbEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            const string collection = "rentify_memory";
            await vectorDbService.DeletePointAsync(collection, notification.EntityId.ToString(), notification.EntityType);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the operation
            logger.LogWarning(ex, "Failed to delete entity {EntityId} of type {EntityType} from vector database. Operation will continue.", 
                notification.EntityId, notification.EntityType);
        }
    }
}

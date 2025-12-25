using Agentic_Rentify.Application.Features.Chat.DTOs;
using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Queries.GetActiveConversations;

public class GetActiveConversationsQueryHandler(IChatRepository chatRepository) 
    : IRequestHandler<GetActiveConversationsQuery, List<ChatConversationDTO>>
{
    public async Task<List<ChatConversationDTO>> Handle(GetActiveConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = request.IsAdmin
            ? await chatRepository.GetActiveConversationsAsync(cancellationToken)
            : await chatRepository.GetUserConversationsAsync(request.UserId ?? string.Empty, cancellationToken);

        return conversations.Select(c => new ChatConversationDTO
        {
            Id = c.Id,
            Subject = c.Subject,
            ClientId = c.ClientId,
            ClientName = c.ClientName,
            ClientEmail = c.ClientEmail,
            AssignedAdminId = c.AssignedAdminId,
            AssignedAdminName = c.AssignedAdminName,
            Status = c.Status.ToString(),
            Priority = c.Priority.ToString(),
            LastMessageAt = c.LastMessageAt
        }).ToList();
    }
}

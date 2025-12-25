using Agentic_Rentify.Application.Features.Chat.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Exceptions;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQueryHandler(IChatRepository chatRepository) : IRequestHandler<GetChatHistoryQuery, List<ChatMessageDTO>>
{
    public async Task<List<ChatMessageDTO>> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
    {
        var conversation = await chatRepository.GetConversationWithMessagesAsync(request.ConversationId, cancellationToken);
        
        if (conversation == null)
        {
            throw new NotFoundException($"Conversation {request.ConversationId} not found");
        }

        return conversation.Messages
            .OrderBy(m => m.SentAt)
            .Select(m => new ChatMessageDTO
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                SenderRole = m.SenderRole.ToString(),
                Content = m.Content,
                SentAt = m.SentAt
            }).ToList();
    }
}

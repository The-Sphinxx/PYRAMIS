using Agentic_Rentify.Application.Features.Chat.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQuery(int conversationId) : IRequest<List<ChatMessageDTO>>
{
    public int ConversationId { get; } = conversationId;
}

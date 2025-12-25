using Agentic_Rentify.Application.Features.Chat.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Queries.GetActiveConversations;

public class GetActiveConversationsQuery : IRequest<List<ChatConversationDTO>>
{
    public string? UserId { get; set; }
    public bool IsAdmin { get; set; }
}

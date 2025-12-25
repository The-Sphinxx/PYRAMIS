using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Interfaces;

public interface IChatService
{
    Task<ChatMessage> SendMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatMessage>> GetHistoryAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatConversation>> GetConversationsAsync(string? userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task<ChatConversation?> GetConversationAsync(int conversationId, string? userId, bool isAdmin, CancellationToken cancellationToken = default);
}

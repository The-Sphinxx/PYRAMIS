using Agentic_Rentify.Core.Entities;

namespace Agentic_Rentify.Application.Interfaces;

public interface IChatRepository
{
    Task<ChatConversation?> GetConversationAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<ChatConversation?> GetConversationWithMessagesAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatConversation>> GetActiveConversationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChatConversation>> GetUserConversationsAsync(string userId, CancellationToken cancellationToken = default);
    Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    Task AddConversationAsync(ChatConversation conversation, CancellationToken cancellationToken = default);
    Task UpdateConversationAsync(ChatConversation conversation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

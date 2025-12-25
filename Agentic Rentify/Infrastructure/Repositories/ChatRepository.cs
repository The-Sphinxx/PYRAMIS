using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Agentic_Rentify.Infrastructure.Repositories;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    public async Task<ChatConversation?> GetConversationAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await context.ChatConversations.FindAsync([conversationId], cancellationToken);
    }

    public async Task<ChatConversation?> GetConversationWithMessagesAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await context.ChatConversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
    }

    public async Task<IReadOnlyList<ChatConversation>> GetActiveConversationsAsync(CancellationToken cancellationToken = default)
    {
        return await context.ChatConversations
            .AsNoTracking()
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChatConversation>> GetUserConversationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.ChatConversations
            .AsNoTracking()
            .Where(c => c.ClientId == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        await context.ChatMessages.AddAsync(message, cancellationToken);
        return message;
    }

    public async Task AddConversationAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
    {
        await context.ChatConversations.AddAsync(conversation, cancellationToken);
    }

    public Task UpdateConversationAsync(ChatConversation conversation, CancellationToken cancellationToken = default)
    {
        context.ChatConversations.Update(conversation);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}

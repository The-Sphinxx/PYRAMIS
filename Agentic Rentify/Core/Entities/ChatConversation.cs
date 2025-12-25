using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Core.Entities;

public class ChatConversation : BaseEntity
{
    public string Subject { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string? AssignedAdminId { get; set; }
    public string? AssignedAdminName { get; set; }
    public ChatStatus Status { get; set; } = ChatStatus.Open;
    public ChatPriority Priority { get; set; } = ChatPriority.Medium;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public List<ChatMessage> Messages { get; set; } = [];
}

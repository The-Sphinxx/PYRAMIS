using Agentic_Rentify.Core.Common;
using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Core.Entities;

public class ChatMessage : BaseEntity
{
    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;

    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public ChatRole SenderRole { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

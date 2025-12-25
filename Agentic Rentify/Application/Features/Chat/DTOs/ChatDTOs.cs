using Agentic_Rentify.Core.Enums;

namespace Agentic_Rentify.Application.Features.Chat.DTOs;

public class ChatMessageDTO
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderRole { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class ChatConversationDTO
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public string? AssignedAdminId { get; set; }
    public string? AssignedAdminName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public List<ChatMessageDTO> Messages { get; set; } = [];
}

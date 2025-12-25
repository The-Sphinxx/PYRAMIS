using Agentic_Rentify.Application.Features.Chat.DTOs;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommand : IRequest<ChatMessageDTO>
{
    public int? ConversationId { get; set; }
    public string? Subject { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public ChatRole SenderRole { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ClientEmail { get; set; }
}

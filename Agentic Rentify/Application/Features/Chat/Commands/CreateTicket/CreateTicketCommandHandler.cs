using Agentic_Rentify.Application.Features.Chat.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Enums;
using MediatR;
using ChatConversationEntity = Agentic_Rentify.Core.Entities.ChatConversation;
using ChatMessageEntity = Agentic_Rentify.Core.Entities.ChatMessage;

namespace Agentic_Rentify.Application.Features.Chat.Commands.CreateTicket;

/// <summary>
/// Handler for creating a new support ticket with strict Client-only authorization
/// </summary>
public class CreateTicketCommandHandler(IChatRepository chatRepository) : IRequestHandler<CreateTicketCommand, ChatConversationDTO>
{
    public async Task<ChatConversationDTO> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        // Validate client role - strict authorization check
        if (string.IsNullOrEmpty(request.ClientRole) || 
            !request.ClientRole.Equals("Client", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("Only clients can create support tickets");
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            throw new BadRequestException("Subject is required");
        }

        if (string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            throw new BadRequestException("Initial message is required");
        }

        // Create new conversation
        var conversation = new ChatConversationEntity
        {
            Subject = request.Subject.Trim(),
            ClientId = request.ClientId,
            ClientName = request.ClientName,
            ClientEmail = request.ClientEmail,
            Status = ChatStatus.Open,
            Priority = ParsePriority(request.Priority),
            LastMessageAt = DateTime.UtcNow
        };

        // Add conversation to repository
        await chatRepository.AddConversationAsync(conversation, cancellationToken);
        await chatRepository.SaveChangesAsync(cancellationToken);

        // Create initial message from client
        var initialMessage = new ChatMessageEntity
        {
            ConversationId = conversation.Id,
            SenderId = request.ClientId,
            SenderName = request.ClientName,
            SenderRole = ChatRole.Client,
            Content = request.InitialMessage.Trim(),
            SentAt = DateTime.UtcNow
        };

        await chatRepository.AddMessageAsync(initialMessage, cancellationToken);
        await chatRepository.SaveChangesAsync(cancellationToken);

        // Map to DTO
        return new ChatConversationDTO
        {
            Id = conversation.Id,
            Subject = conversation.Subject,
            ClientId = conversation.ClientId,
            ClientName = conversation.ClientName,
            ClientEmail = conversation.ClientEmail,
            Status = conversation.Status.ToString(),
            Priority = conversation.Priority.ToString(),
            LastMessageAt = conversation.LastMessageAt,
            Messages = new List<ChatMessageDTO>
            {
                new ChatMessageDTO
                {
                    Id = initialMessage.Id,
                    ConversationId = initialMessage.ConversationId,
                    SenderId = initialMessage.SenderId,
                    SenderName = initialMessage.SenderName,
                    SenderRole = initialMessage.SenderRole.ToString(),
                    Content = initialMessage.Content,
                    SentAt = initialMessage.SentAt
                }
            }
        };
    }

    /// <summary>
    /// Parse priority string to ChatPriority enum
    /// </summary>
    private ChatPriority ParsePriority(string priority)
    {
        return priority?.ToLower() switch
        {
            "high" => ChatPriority.High,
            "low" => ChatPriority.Low,
            _ => ChatPriority.Medium
        };
    }
}

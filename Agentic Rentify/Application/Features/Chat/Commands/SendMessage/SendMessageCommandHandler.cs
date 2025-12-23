using Agentic_Rentify.Application.Features.Chat.DTOs;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Enums;
using Agentic_Rentify.Core.Exceptions;
using MediatR;
using ChatMessageEntity = Agentic_Rentify.Core.Entities.ChatMessage;
using ChatConversationEntity = Agentic_Rentify.Core.Entities.ChatConversation;

namespace Agentic_Rentify.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandHandler(IChatRepository chatRepository) : IRequestHandler<SendMessageCommand, ChatMessageDTO>
{
    public async Task<ChatMessageDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        ChatConversationEntity? conversation = null;

        if (request.ConversationId.HasValue)
        {
            conversation = await chatRepository.GetConversationAsync(request.ConversationId.Value, cancellationToken);
            if (conversation == null)
            {
                throw new NotFoundException($"Conversation {request.ConversationId.Value} not found");
            }
        }
        else
        {
            // New conversation
            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                throw new BadRequestException("Subject is required for new conversations");
            }

            conversation = new ChatConversationEntity
            {
                Subject = request.Subject!,
                ClientId = request.SenderRole == ChatRole.Client ? request.SenderId : string.Empty,
                ClientName = request.SenderRole == ChatRole.Client ? request.SenderName : string.Empty,
                ClientEmail = request.ClientEmail ?? string.Empty,
                Status = ChatStatus.Open,
                Priority = ChatPriority.Medium,
                LastMessageAt = DateTime.UtcNow
            };

            await chatRepository.AddConversationAsync(conversation, cancellationToken);
            await chatRepository.SaveChangesAsync(cancellationToken);
        }

        var message = new ChatMessageEntity
        {
            ConversationId = conversation.Id,
            SenderId = request.SenderId,
            SenderName = request.SenderName,
            SenderRole = request.SenderRole,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        await chatRepository.AddMessageAsync(message, cancellationToken);

        conversation.LastMessageAt = message.SentAt;
        await chatRepository.UpdateConversationAsync(conversation, cancellationToken);
        await chatRepository.SaveChangesAsync(cancellationToken);

        return new ChatMessageDTO
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = message.SenderName,
            SenderRole = message.SenderRole.ToString(),
            Content = message.Content,
            SentAt = message.SentAt
        };
    }
}

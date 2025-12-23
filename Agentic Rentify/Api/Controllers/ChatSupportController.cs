using Agentic_Rentify.Application.Features.Chat.Commands.CreateTicket;
using Agentic_Rentify.Application.Features.Chat.Commands.SendMessage;
using Agentic_Rentify.Application.Features.Chat.Queries.GetActiveConversations;
using Agentic_Rentify.Application.Features.Chat.Queries.GetChatHistory;
using Agentic_Rentify.Infragentic.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Real-time chat system for Client-Admin communication (REST endpoints, see ChatHub for WebSocket)
/// </summary>
[ApiController]
[Route("api/chat")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Chat System")]
public class ChatSupportController(IMediator mediator, IHubContext<ChatHub> hubContext) : ControllerBase
{
    /// <summary>
    /// Get all active conversations (Admin sees all, Client sees only their own)
    /// </summary>
    [HttpGet("conversations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var isAdmin = role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;

        var conversations = await mediator.Send(new GetActiveConversationsQuery
        {
            UserId = userId,
            IsAdmin = isAdmin
        });

        return Ok(conversations);
    }

    /// <summary>
    /// Get chat history for a specific conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(int conversationId)
    {
        var messages = await mediator.Send(new GetChatHistoryQuery(conversationId));
        return Ok(messages);
    }

    /// <summary>
    /// Send a message (creates conversation if new)
    /// </summary>
    [HttpPost("messages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "User";
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        command.SenderId = userId;
        command.SenderName = userName;
        command.SenderRole = role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true 
            ? Agentic_Rentify.Core.Enums.ChatRole.Admin 
            : Agentic_Rentify.Core.Enums.ChatRole.Client;

        var message = await mediator.Send(command);
        return Ok(message);
    }

    /// <summary>
    /// Create a new support ticket (Client-only endpoint)
    /// Clients can initiate a support conversation with an initial message.
    /// </summary>
    /// <remarks>
    /// This endpoint is restricted to users with the "Client" role.
    /// The ticket will be immediately visible to all admins via SignalR.
    /// </remarks>
    [HttpPost("tickets")]
    [Authorize(Roles = "Client")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "Client";
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        // Populate command with user context
        command.ClientId = userId;
        command.ClientName = userName;
        command.ClientEmail = userEmail;
        command.ClientRole = role ?? "Client";

        try
        {
            var conversation = await mediator.Send(command);
            
            // Broadcast new ticket notification to all connected Admins via SignalR
            await hubContext.Clients
                .Group("Admins")
                .SendAsync("NewTicketCreated", new
                {
                    id = conversation.Id,
                    subject = conversation.Subject,
                    clientId = conversation.ClientId,
                    clientName = conversation.ClientName,
                    clientEmail = conversation.ClientEmail,
                    status = conversation.Status,
                    priority = conversation.Priority,
                    lastMessageAt = conversation.LastMessageAt,
                    messageCount = conversation.Messages?.Count ?? 0
                });

            return Ok(new
            {
                message = "Ticket created successfully",
                data = conversation
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}


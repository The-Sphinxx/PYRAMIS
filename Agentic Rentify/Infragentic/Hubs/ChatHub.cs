using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Agentic_Rentify.Infragentic.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IAgentLogRepository _agentLogRepository;

    public ChatHub(IAgentLogRepository agentLogRepository)
    {
        _agentLogRepository = agentLogRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(role))
        {
            // Add to role-specific group
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Client_{userId}");
            }

            await LogActivity(userId, "Connected", $"Role: {role}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await LogActivity(userId, "Disconnected", exception?.Message ?? "Clean disconnect");
        }
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send a message from client to admin or vice versa with strict role enforcement
    /// </summary>
    public async Task SendMessage(int conversationId, string content)
    {
        var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var senderName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        var senderRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(senderRole))
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized: Missing user identity");
            return;
        }

        // Determine role enum
        var roleEnum = senderRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) 
            ? ChatRole.Admin 
            : ChatRole.Client;

        // Build message object
        var message = new
        {
            ConversationId = conversationId,
            SenderId = senderId,
            SenderName = senderName,
            SenderRole = roleEnum.ToString(),
            Content = content,
            SentAt = DateTime.UtcNow
        };

        // Broadcast logic
        if (roleEnum == ChatRole.Admin)
        {
            // Admin -> Send to specific client group (based on conversation client)
            // We need the clientId from the conversation - for now, broadcast to all clients in conversation
            await Clients.Group($"Conversation_{conversationId}").SendAsync("ReceiveMessage", message);
        }
        else
        {
            // Client -> Send to all admins
            await Clients.Group("Admins").SendAsync("ReceiveMessage", message);
            // Also echo back to sender
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        await LogActivity(senderId, "SentMessage", $"ConversationId: {conversationId}, Role: {roleEnum}");
    }

    /// <summary>
    /// Notify all admins of a new ticket created by a client (called from CreateTicket endpoint)
    /// </summary>
    public async Task NotifyNewTicket(object ticketData)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        // Only clients should call this (or allow via authorization check)
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
        {
            await Clients.Caller.SendAsync("Error", "Unauthorized: Missing user identity");
            return;
        }

        if (!role.Equals("Client", StringComparison.OrdinalIgnoreCase))
        {
            await Clients.Caller.SendAsync("Error", "Only clients can create tickets");
            return;
        }

        // Broadcast to all admins
        await Clients.Group("Admins").SendAsync("NewTicketCreated", ticketData);

        await LogActivity(userId, "CreatedTicket", $"Ticket: {ticketData}");
    }

    /// <summary>
    /// Join a conversation group (for targeted messaging)
    /// </summary>
    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        await LogActivity(
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            "JoinedConversation",
            $"ConversationId: {conversationId}"
        );
    }

    /// <summary>
    /// Leave a conversation group
    /// </summary>
    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        await LogActivity(
            Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown",
            "LeftConversation",
            $"ConversationId: {conversationId}"
        );
    }

    /// <summary>
    /// Notify typing indicator
    /// </summary>
    public async Task NotifyTyping(int conversationId, bool isTyping)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "User";

        await Clients.OthersInGroup($"Conversation_{conversationId}")
            .SendAsync("UserTyping", new { UserId = userId, UserName = userName, IsTyping = isTyping });
    }

    private async Task LogActivity(string userId, string action, string details)
    {
        try
        {
            var log = new Agentic_Rentify.Core.Entities.AgentExecutionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PluginName = "ChatHub",
                FunctionName = action,
                ArgumentsJson = $"{{ \"details\": \"{details}\" }}",
                ResultJson = string.Empty,
                IsError = false,
                ErrorMessage = null,
                ExecutionDurationMs = 0,
                Timestamp = DateTime.UtcNow
            };

            await _agentLogRepository.SaveLogAsync(log);
        }
        catch
        {
            // Silent fail to not disrupt chat flow
        }
    }
}


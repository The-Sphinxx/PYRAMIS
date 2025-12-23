using Agentic_Rentify.Application.Features.Chat.DTOs;
using MediatR;

namespace Agentic_Rentify.Application.Features.Chat.Commands.CreateTicket;

/// <summary>
/// Command to create a new support ticket (Client-only)
/// </summary>
public class CreateTicketCommand : IRequest<ChatConversationDTO>
{
    /// <summary>
    /// The subject/title of the support ticket
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Priority level of the ticket
    /// </summary>
    public string Priority { get; set; } = "Medium";

    /// <summary>
    /// Initial message content for the ticket
    /// </summary>
    public string InitialMessage { get; set; } = string.Empty;

    /// <summary>
    /// Client ID (extracted from JWT claims) - set by controller
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client name (extracted from JWT claims) - set by controller
    /// </summary>
    public string ClientName { get; set; } = string.Empty;

    /// <summary>
    /// Client email (extracted from JWT claims) - set by controller
    /// </summary>
    public string ClientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Client role (should be "Client") - set by controller for validation
    /// </summary>
    public string ClientRole { get; set; } = string.Empty;
}

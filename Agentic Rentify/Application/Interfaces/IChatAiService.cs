namespace Agentic_Rentify.Application.Interfaces;

public interface IChatAiService
{
    Task<string> GetResponseAsync(string userMessage, string? userId = null, List<AiChatMessage>? conversationHistory = null);
}

public class AiChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

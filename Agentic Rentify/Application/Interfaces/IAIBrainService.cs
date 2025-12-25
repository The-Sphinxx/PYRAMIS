using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agentic_Rentify.Application.Interfaces;

/// <summary>
/// Represents the three different AI "brains" used in the system.
/// Each brain has different characteristics and use cases.
/// </summary>
public enum AIBrain
{
    /// <summary>
    /// Default Brain - Balanced performance and cost. Used as fallback.
    /// Model: openai/gpt-3.5-turbo
    /// </summary>
    DefaultBrain,

    /// <summary>
    /// Fast Brain - Optimized for fast structured JSON generation.
    /// Used specifically for trip planning and itinerary generation.
    /// Model: openai/gpt-oss-safeguard-20b
    /// </summary>
    FastBrain,

    /// <summary>
    /// Smart Brain - Most intelligent and capable model.
    /// Used for complex reasoning and decision-making.
    /// Model: x-ai/grok-4.1-fast
    /// </summary>
    SmartBrain
}

/// <summary>
/// Service interface for managing multiple AI models with fallback logic.
/// Implements a resilient three-brain strategy with automatic failover.
/// </summary>
public interface IAIBrainService
{
    /// <summary>
    /// Invokes an AI function with the specified brain, with automatic fallback to DefaultBrain.
    /// </summary>
    /// <param name="brain">The primary AI brain to use</param>
    /// <param name="function">The kernel function to invoke</param>
    /// <param name="arguments">The kernel arguments</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The function result</returns>
    /// <exception cref="Exception">Thrown if all brains fail</exception>
    Task<FunctionResult> InvokeAsync(
        AIBrain brain,
        KernelFunction function,
        KernelArguments arguments,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes an AI function with fallback logic.
    /// Tries primary brain first, falls back to DefaultBrain on failure.
    /// </summary>
    /// <param name="brain">The primary AI brain to use</param>
    /// <param name="function">The kernel function to invoke</param>
    /// <param name="arguments">The kernel arguments</param>
    /// <param name="onFallback">Callback invoked when falling back to DefaultBrain</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The function result</returns>
    Task<FunctionResult> InvokeWithFallbackAsync(
        AIBrain brain,
        KernelFunction function,
        KernelArguments arguments,
        Action<AIBrain, Exception>? onFallback = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the chat completion service for a specific brain.
    /// </summary>
    /// <param name="brain">The AI brain to get the service for</param>
    /// <returns>The chat completion service</returns>
    IChatCompletionService GetChatCompletionService(AIBrain brain);

    /// <summary>
    /// Gets the model ID for a specific brain.
    /// </summary>
    /// <param name="brain">The AI brain</param>
    /// <returns>The model ID (e.g., "openai/gpt-3.5-turbo")</returns>
    string GetModelId(AIBrain brain);
}

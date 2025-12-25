using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Infragentic.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Agentic_Rentify.Infragentic.Services;

/// <summary>
/// Implementation of IAIBrainService with three-brain strategy and fallback logic.
/// Manages DefaultBrain, FastBrain, and SmartBrain with automatic failover.
/// </summary>
public class AIBrainService : IAIBrainService
{
    private readonly Kernel _kernel;
    private readonly AiSettings _aiSettings;
    private readonly ILogger<AIBrainService> _logger;
    private readonly Dictionary<AIBrain, IChatCompletionService> _brainServices;
    private readonly Dictionary<AIBrain, string> _modelMap;

    public AIBrainService(
        Kernel kernel,
        IOptions<AiSettings> aiSettings,
        ILogger<AIBrainService> logger)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _aiSettings = aiSettings.Value ?? throw new ArgumentNullException(nameof(aiSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize brain services
        _brainServices = new Dictionary<AIBrain, IChatCompletionService>();
        _modelMap = new Dictionary<AIBrain, string>
        {
            { AIBrain.DefaultBrain, _aiSettings.DefaultModel },
            { AIBrain.FastBrain, _aiSettings.PlannerModel },
            { AIBrain.SmartBrain, _aiSettings.ChatModel }
        };

        _logger.LogInformation("AIBrainService initialized with three brains: DefaultBrain={DefaultModel}, FastBrain={PlannerModel}, SmartBrain={ChatModel}",
            _aiSettings.DefaultModel, _aiSettings.PlannerModel, _aiSettings.ChatModel);
    }

    /// <summary>
    /// Invokes a kernel function with the specified brain.
    /// </summary>
    public async Task<FunctionResult> InvokeAsync(
        AIBrain brain,
        KernelFunction function,
        KernelArguments arguments,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invoking function {Function} with {Brain}", function.Name, brain);

        try
        {
            var result = await _kernel.InvokeAsync(function, arguments, cancellationToken);
            _logger.LogInformation("Function {Function} succeeded with {Brain}", function.Name, brain);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Function {Function} failed with {Brain}: {Message}", function.Name, brain, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Invokes a kernel function with fallback to DefaultBrain.
    /// If the primary brain fails (timeout, API error), automatically retry with DefaultBrain.
    /// </summary>
    public async Task<FunctionResult> InvokeWithFallbackAsync(
        AIBrain brain,
        KernelFunction function,
        KernelArguments arguments,
        Action<AIBrain, Exception>? onFallback = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invoking function {Function} with {Brain} (fallback enabled)", function.Name, brain);

        try
        {
            var result = await _kernel.InvokeAsync(function, arguments, cancellationToken);
            _logger.LogInformation("Function {Function} succeeded with {Brain}", function.Name, brain);
            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable || 
                                               ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning(ex, "Function {Function} with {Brain} hit rate limit or service unavailable. Falling back to DefaultBrain", 
                function.Name, brain);

            onFallback?.Invoke(brain, ex);

            // Only fallback if not already using DefaultBrain
            if (brain == AIBrain.DefaultBrain)
            {
                throw;
            }

            try
            {
                var fallbackResult = await _kernel.InvokeAsync(function, arguments, cancellationToken);
                _logger.LogInformation("Fallback to DefaultBrain succeeded for function {Function}", function.Name);
                return fallbackResult;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Fallback to DefaultBrain also failed for function {Function}", function.Name);
                throw new InvalidOperationException(
                    $"Both {brain} and DefaultBrain failed to invoke function {function.Name}. " +
                    $"Primary error: {ex.Message}. Fallback error: {fallbackEx.Message}", fallbackEx);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Function {Function} with {Brain} timed out. Falling back to DefaultBrain", 
                function.Name, brain);

            onFallback?.Invoke(brain, ex);

            if (brain == AIBrain.DefaultBrain)
            {
                throw;
            }

            try
            {
                var fallbackResult = await _kernel.InvokeAsync(function, arguments, cancellationToken);
                _logger.LogInformation("Fallback to DefaultBrain succeeded for function {Function}", function.Name);
                return fallbackResult;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Fallback to DefaultBrain also failed for function {Function}", function.Name);
                throw new InvalidOperationException(
                    $"Both {brain} and DefaultBrain failed to invoke function {function.Name} due to timeout. " +
                    $"Primary error: {ex.Message}. Fallback error: {fallbackEx.Message}", fallbackEx);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Function {Function} with {Brain} failed unexpectedly. Falling back to DefaultBrain", 
                function.Name, brain);

            onFallback?.Invoke(brain, ex);

            if (brain == AIBrain.DefaultBrain)
            {
                throw;
            }

            try
            {
                var fallbackResult = await _kernel.InvokeAsync(function, arguments, cancellationToken);
                _logger.LogInformation("Fallback to DefaultBrain succeeded for function {Function}", function.Name);
                return fallbackResult;
            }
            catch (Exception fallbackEx)
            {
                _logger.LogError(fallbackEx, "Fallback to DefaultBrain also failed for function {Function}", function.Name);
                throw new InvalidOperationException(
                    $"Both {brain} and DefaultBrain failed to invoke function {function.Name}. " +
                    $"Primary error: {ex.Message}. Fallback error: {fallbackEx.Message}", fallbackEx);
            }
        }
    }

    /// <summary>
    /// Gets the chat completion service for a specific brain.
    /// </summary>
    public IChatCompletionService GetChatCompletionService(AIBrain brain)
    {
        if (_brainServices.TryGetValue(brain, out var service))
        {
            return service;
        }

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();
        _brainServices[brain] = chatService;
        return chatService;
    }

    /// <summary>
    /// Gets the model ID for a specific brain.
    /// </summary>
    public string GetModelId(AIBrain brain)
    {
        return _modelMap.TryGetValue(brain, out var modelId) 
            ? modelId 
            : throw new ArgumentException($"Unknown brain: {brain}");
    }
}

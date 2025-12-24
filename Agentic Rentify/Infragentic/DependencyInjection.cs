using Agentic_Rentify.Infragentic.Settings;
using Agentic_Rentify.Infragentic.Plugins;
using Agentic_Rentify.Infragentic.Filters;
using Agentic_Rentify.Infragentic.Services;
using Agentic_Rentify.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Qdrant.Client;
using Microsoft.Extensions.Options;

namespace Agentic_Rentify.Infragentic;

public static class InfragenticExtensions
{
    public static IServiceCollection AddInfragenticServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiSettings>(configuration.GetSection("AI")); // [cite: 4]
        var aiSettings = configuration.GetSection("AI").Get<AiSettings>()!;

        services.AddHttpClient("OpenRouter", client =>
        {
            client.BaseAddress = new Uri(aiSettings.OpenAIEndpoint); // [cite: 5]
            client.Timeout = TimeSpan.FromMinutes(3); // لحل مشكلة ResponseEnded
        });

        services.AddSingleton<Kernel>(sp =>
        {
            var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("OpenRouter");
            var builder = Kernel.CreateBuilder();

            // تسجيل الأدمغة بـ IDs فريدة [cite: 6, 7, 8, 9]
            builder.AddOpenAIChatCompletion(aiSettings.ChatModel, aiSettings.OpenAIKey, httpClient: httpClient, serviceId: "SmartBrain");
            builder.AddOpenAIChatCompletion(aiSettings.PlannerModel, aiSettings.OpenAIKey, httpClient: httpClient, serviceId: "FastBrain");
            builder.AddOpenAIChatCompletion(aiSettings.DefaultModel, aiSettings.OpenAIKey, httpClient: httpClient, serviceId: "DefaultBrain");

            // فلتر المراقبة والتسجيل في DB [cite: 10, 474]
            builder.Services.AddSingleton<IFunctionInvocationFilter>(new AgentInvocationFilter(sp.GetRequiredService<IServiceScopeFactory>()));

            // تسجيل الـ Plugins [cite: 11, 12, 13]
            builder.Plugins.AddFromObject(sp.GetRequiredService<SqlQueryPlugin>(), "SqlQuery");
            builder.Plugins.AddFromObject(new DiscoveryPlugin(sp.GetRequiredService<IServiceScopeFactory>()), "Discovery");
            builder.Plugins.AddFromObject(new TravelDatabasePlugin(sp.GetRequiredService<IServiceScopeFactory>()), "TravelDatabase");
            builder.Plugins.AddFromObject(new BookingPlugin(sp.GetRequiredService<IServiceScopeFactory>()), "Booking");

            return builder.Build();
        });

        // تسجيل الخدمات الأساسية [cite: 14, 15]
        services.AddSingleton<QdrantClient>(sp => new QdrantClient(aiSettings.QdrantHost, aiSettings.QdrantPort, https: false));
        services.AddScoped<IVectorDbService, QdrantVectorService>();
        services.AddSingleton<SqlQueryPlugin>();
        services.AddScoped<IAIBrainService, AIBrainService>();
        services.AddScoped<IChatAiService, ChatAiService>();
        services.AddScoped<IAiTripPlannerService, SmartAiTripPlannerService>(); // المخطط الوحيد المعتمد [cite: 16]

        return services;
    }
}
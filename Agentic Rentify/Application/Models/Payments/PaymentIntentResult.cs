namespace Agentic_Rentify.Application.Models.Payments;

public record PaymentIntentResult(
    string ClientSecret,
    string PaymentIntentId,
    string PublishableKey
);

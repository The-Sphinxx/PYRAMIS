using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Application.Models.Payments;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;

namespace Agentic_Rentify.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly StripeSettings _settings;
    private readonly string _clientAppUrl;

    public StripePaymentService(IOptions<StripeSettings> stripeOptions, IConfiguration configuration)
    {
        _settings = stripeOptions.Value;
        _clientAppUrl = configuration["ClientAppUrl"] ?? configuration["AppBaseUrl"] ?? "http://localhost:3000";
        StripeConfiguration.ApiKey = _settings.SecretKey;
    }

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(Booking booking)
    {
        var intentOptions = new PaymentIntentCreateOptions
        {
            Amount = (long)Math.Round(booking.TotalPrice * 100M, 0, MidpointRounding.AwayFromZero),
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                { "bookingId", booking.Id.ToString() },
                { "userId", booking.UserId },
                { "bookingType", booking.BookingType },
                { "entityId", booking.EntityId.ToString() }
            }
        };

        var intentService = new PaymentIntentService();
        var intent = await intentService.CreateAsync(intentOptions);

        booking.StripeSessionId = intent.Id; // Stores PaymentIntent id for webhook lookup

        return new PaymentIntentResult(
            intent.ClientSecret,
            intent.Id,
            _settings.PublishableKey
        );
    }
}

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingResult
{
    public int BookingId { get; init; }
    public string ClientSecret { get; init; } = string.Empty;
    public string PaymentIntentId { get; init; } = string.Empty;
    public string PublishableKey { get; init; } = string.Empty;
}

using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommand(string paymentIntentId) : IRequest<Unit>
{
    public string PaymentIntentId { get; } = paymentIntentId;
}

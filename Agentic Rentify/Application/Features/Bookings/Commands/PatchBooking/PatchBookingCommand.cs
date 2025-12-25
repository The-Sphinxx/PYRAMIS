using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.PatchBooking;

public class PatchBookingCommand : IRequest<int>
{
    public int Id { get; set; }
    public string? Status { get; set; }
    public string? PaymentStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalPrice { get; set; }
}

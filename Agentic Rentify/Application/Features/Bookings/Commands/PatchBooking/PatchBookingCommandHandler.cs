using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Exceptions;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.PatchBooking;

public class PatchBookingCommandHandler : IRequestHandler<PatchBookingCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public PatchBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(PatchBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.Id);
        if (booking == null)
            throw new NotFoundException($"Booking with ID {request.Id} not found");

        // Update only provided fields
        if (request.Status != null)
            booking.Status = request.Status;

        if (request.PaymentStatus != null)
            booking.PaymentStatus = request.PaymentStatus;

        if (request.StartDate.HasValue)
            booking.StartDate = request.StartDate.Value;

        if (request.EndDate.HasValue)
            booking.EndDate = request.EndDate.Value;

        if (request.TotalPrice.HasValue)
            booking.TotalPrice = request.TotalPrice.Value;

        _unitOfWork.Repository<Booking>().Update(booking);
        await _unitOfWork.SaveChangesAsync();

        return booking.Id;
    }
}

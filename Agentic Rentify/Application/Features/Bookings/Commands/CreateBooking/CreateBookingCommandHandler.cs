using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using MediatR;

namespace Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(IUnitOfWork unitOfWork, IPaymentService paymentService)
    : IRequestHandler<CreateBookingCommand, CreateBookingResult>
{
    public async Task<CreateBookingResult> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = new Booking
        {
            UserId = request.UserId,
            EntityId = request.EntityId,
            BookingType = request.BookingType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPrice = request.TotalPrice,
            Status = BookingStatus.Pending,
            IsPaid = false
        };

        await unitOfWork.Repository<Booking>().AddAsync(booking);
        await unitOfWork.CompleteAsync();

        var paymentIntent = await paymentService.CreatePaymentIntentAsync(booking);

        await unitOfWork.Repository<Booking>().UpdateAsync(booking);
        await unitOfWork.CompleteAsync();

        return new CreateBookingResult
        {
            BookingId = booking.Id,
            ClientSecret = paymentIntent.ClientSecret,
            PaymentIntentId = paymentIntent.PaymentIntentId,
            PublishableKey = paymentIntent.PublishableKey
        };
    }
}

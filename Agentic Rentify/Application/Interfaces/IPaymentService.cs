using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Application.Models.Payments;

namespace Agentic_Rentify.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResult> CreatePaymentIntentAsync(Booking booking);
}

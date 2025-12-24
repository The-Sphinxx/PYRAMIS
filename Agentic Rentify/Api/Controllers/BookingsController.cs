using Agentic_Rentify.Application.Features.Bookings.Commands.ConfirmBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.UpdateBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.DeleteBooking;
using Agentic_Rentify.Application.Features.Bookings.Commands.CancelBooking;
using Agentic_Rentify.Application.Features.Bookings.Queries.GetActiveBookings;
using Agentic_Rentify.Application.Features.Bookings.Queries.GetBookingById;
using Agentic_Rentify.Application.Features.Bookings.Commands.CreateBooking;
using Agentic_Rentify.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// Booking management and Stripe payment integration
/// </summary>
/// <remarks>
/// Handles the complete booking lifecycle from creation to payment confirmation.
/// Integrates with Stripe Payment Intents + Elements for secure payment processing.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Booking System")]
public class BookingsController(IMediator mediator, IOptions<StripeSettings> stripeOptions, ILogger<BookingsController> logger) : ControllerBase
{
    private readonly StripeSettings _stripeSettings = stripeOptions.Value;

    /// <summary>
    /// Gets all active bookings (excludes cancelled/failed/soft-deleted).
    /// </summary>
    /// <returns>List of active bookings with entity details</returns>
    /// <response code="200">Returns list of active bookings</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        var items = await mediator.Send(new GetActiveBookingsQuery());
        return Ok(items);
    }

    /// <summary>
    /// Get a booking by ID.
    /// </summary>
    /// <param name="id">Booking identifier</param>
    /// <returns>Booking details including payment status</returns>
    /// <response code="200">Returns the booking</response>
    /// <response code="404">Booking not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await mediator.Send(new GetBookingByIdQuery(id));
        return Ok(item);
    }

    /// <summary>
    /// Creates a new booking and initializes a Stripe PaymentIntent (for Payment Element).
    /// </summary>
    /// <param name="command">The booking command containing user, entity, type, dates, and total price.</param>
    /// <returns>A JSON response with the Stripe client secret for Payment Element.</returns>
    /// <remarks>
    /// **Request Body:**
    /// ```json
    /// {
    ///   "userId": "user-guid-here",
    ///   "entityId": 5,
    ///   "bookingType": "Trip",
    ///   "startDate": "2025-12-25T00:00:00Z",
    ///   "endDate": "2025-12-30T00:00:00Z",
    ///   "totalPrice": 1500.00
    /// }
    /// ```
    /// 
    /// **Booking Types:** `Trip`, `Hotel`, `Car`, `Attraction`
    /// 
    /// The booking is initially created with status "Pending" and IsPaid=false.
    /// The returned clientSecret is used to render Stripe Payment Element on the frontend.
    /// 
    /// **Payment Flow:**
    /// 1. Frontend calls this endpoint
    /// 2. Frontend mounts Stripe Payment Element with the provided clientSecret
    /// 3. User completes payment on your hosted page (Elements renders secure iframes)
    /// 4. Stripe webhooks trigger booking confirmation
    /// 5. Use GET /api/Bookings/{id} to verify payment status
    /// </remarks>
    /// <response code="200">Returns the PaymentIntent client secret for Stripe Elements.</response>
    /// <response code="400">Validation failed or Stripe API error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(new CreateBookingResponse
        {
            BookingId = result.BookingId,
            ClientSecret = result.ClientSecret,
            PaymentIntentId = result.PaymentIntentId,
            PublishableKey = result.PublishableKey
        });
    }

    /// <summary>
    /// Update booking dates/price.
    /// </summary>
    /// <param name="id">Booking ID</param>
    /// <param name="command">Updated booking details</param>
    /// <response code="200">Booking updated successfully</response>
    /// <response code="400">ID mismatch or validation error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingCommand command)
    {
        if (id != command.Id) return BadRequest(new { message = "ID mismatch" });
        var resultId = await mediator.Send(command);
        return Ok(new { id = resultId });
    }

    /// <summary>
    /// Cancel a booking. If already paid, marks as Cancelled (refund not implemented).
    /// </summary>
    /// <param name="id">Booking ID to cancel</param>
    /// <remarks>
    /// Note: This operation does not automatically process refunds through Stripe.
    /// Refund logic should be implemented separately if needed.
    /// </remarks>
    /// <response code="200">Booking cancelled successfully</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id)
    {
        await mediator.Send(new CancelBookingCommand(id));
        return Ok(new { id, status = "cancelled" });
    }

    /// <summary>
    /// Soft-delete a booking (marks as deleted and cancelled).
    /// </summary>
    /// <param name="id">Booking ID to delete</param>
    /// <response code="200">Booking soft-deleted successfully</response>
    /// <response code="404">Booking not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await mediator.Send(new DeleteBookingCommand(id));
        return Ok(new { id, deleted = true });
    }

    /// <summary>
    /// Stripe webhook endpoint for payment confirmation.
    /// </summary>
    /// <remarks>
    /// This endpoint is called by Stripe after successful payment.
    /// Verifies webhook signature and updates booking status to Confirmed with IsPaid=true.
    /// 
    /// **Webhook Configuration:**
    /// - Event Type: `payment_intent.succeeded`
    /// - Signature Verification: Required using WebhookSecret
    /// - Endpoint URL: https://yourapi.com/api/bookings/webhook
    /// 
    /// **Flow:**
    /// 1. Stripe sends POST request to this endpoint
    /// 2. Validates signature using Stripe-Signature header
    /// 3. Extracts payment_intent ID from event payload
    /// 4. Confirms booking payment status
    /// </remarks>
    /// <response code="200">Webhook received and processed successfully.</response>
    /// <response code="400">Invalid webhook signature or malformed payload.</response>
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleWebhook()
    {
        var payload = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"]; // Stripe sends lower case header name

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signatureHeader, _stripeSettings.WebhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning(ex, "Stripe webhook signature validation failed.");
            return BadRequest(new { message = "Invalid webhook signature" });
        }

        if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded && stripeEvent.Data.Object is PaymentIntent intent)
        {
            await mediator.Send(new ConfirmBookingCommand(intent.Id));
        }

        return Ok(new { received = true });
    }
}

/// <summary>
/// Response from create booking with Stripe PaymentIntent client secret
/// </summary>
public class CreateBookingResponse
{
    /// <summary>Database identifier for the booking</summary>
    public int BookingId { get; set; }

    /// <summary>Stripe client secret used to render Payment Element</summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>Stripe PaymentIntent ID (stored server-side)</summary>
    public string PaymentIntentId { get; set; } = string.Empty;

    /// <summary>Publishable key to initialize Stripe.js on the client</summary>
    public string PublishableKey { get; set; } = string.Empty;
}

using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Exceptions;
using MediatR;

namespace Agentic_Rentify.Application.Features.Profile.Queries.GetUserProfile;

public record GetUserProfileQuery(string UserId) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IGenericRepository<Booking> _bookingRepository;
    private readonly IGenericRepository<WishlistItem> _wishlistRepository;
    private readonly IGenericRepository<Trip> _tripRepository;
    private readonly IGenericRepository<Hotel> _hotelRepository;
    private readonly IGenericRepository<Car> _carRepository;
    private readonly IGenericRepository<Attraction> _attractionRepository;

    public GetUserProfileQueryHandler(
        IIdentityService identityService,
        IGenericRepository<Booking> bookingRepository,
        IGenericRepository<WishlistItem> wishlistRepository,
        IGenericRepository<Trip> tripRepository,
        IGenericRepository<Hotel> hotelRepository,
        IGenericRepository<Car> carRepository,
        IGenericRepository<Attraction> attractionRepository)
    {
        _identityService = identityService;
        _bookingRepository = bookingRepository;
        _wishlistRepository = wishlistRepository;
        _tripRepository = tripRepository;
        _hotelRepository = hotelRepository;
        _carRepository = carRepository;
        _attractionRepository = attractionRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var bookings = await _bookingRepository.GetAsync(b => b.UserId == request.UserId && !b.IsDeleted);
        var wishlist = await _wishlistRepository.GetAsync(w => w.UserId == request.UserId && !w.IsDeleted);

        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        // Enrich bookings with entity details
        var enrichedBookings = new List<BookingDto>();
        foreach (var booking in bookings)
        {
            var bookingDto = await EnrichBookingAsync(booking, cancellationToken);
            if (bookingDto != null)
            {
                enrichedBookings.Add(bookingDto);
            }
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = string.IsNullOrWhiteSpace(fullName) ? user.UserName ?? string.Empty : fullName,
            ProfileImage = user.ProfileImage ?? string.Empty,
            Phone = user.PhoneNumber,
            Nationality = user.Nationality,
            Gender = user.Gender,
            DateOfBirth = user.DateOfBirth,
            MembershipType = "Standard Member",
            Bookings = enrichedBookings,
            Wishlist = wishlist
                .Select(w => new WishlistItemDto
                {
                    Id = w.Id,
                    ItemId = w.ItemId,
                    ItemType = w.ItemType,
                    Title = w.Title,
                    ImageUrl = w.ImageUrl,
                    Location = w.Location,
                    Rating = w.Rating
                })
                .ToList()
        };
    }

    private async Task<BookingDto?> EnrichBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        string title = string.Empty;
        string image = string.Empty;
        string location = string.Empty;

        try
        {
            switch (booking.BookingType.ToLower())
            {
                case "trip":
                    var trip = await _tripRepository.GetByIdAsync(booking.EntityId);
                    if (trip != null)
                    {
                        title = trip.Title;
                        image = trip.MainImage;
                        location = trip.Destination;
                    }
                    break;

                case "hotel":
                    var hotel = await _hotelRepository.GetByIdAsync(booking.EntityId);
                    if (hotel != null)
                    {
                        title = hotel.Name;
                        image = hotel.Images?.FirstOrDefault() ?? string.Empty;
                        location = hotel.City;
                    }
                    break;

                case "car":
                    var car = await _carRepository.GetByIdAsync(booking.EntityId);
                    if (car != null)
                    {
                        title = car.Name;
                        image = car.Images?.FirstOrDefault() ?? string.Empty;
                        location = car.City;
                    }
                    break;

                case "attraction":
                    var attraction = await _attractionRepository.GetByIdAsync(booking.EntityId);
                    if (attraction != null)
                    {
                        title = attraction.Name;
                        image = attraction.Images?.FirstOrDefault()?.Url ?? string.Empty;
                        location = attraction.City;
                    }
                    break;
            }

            // If entity not found or deleted, skip this booking
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }

            return new BookingDto
            {
                Id = booking.Id,
                EntityId = booking.EntityId,
                Type = booking.BookingType,
                Title = title,
                Image = image,
                Location = location,
                Date = booking.StartDate.ToString("MMM dd, yyyy"),
                Time = booking.StartDate.ToString("hh:mm tt"),
                Reference = $"BK{booking.Id:D6}",
                Status = booking.Status.ToString(),
                TotalPrice = booking.TotalPrice
            };
        }
        catch
        {
            // If any error occurs, skip this booking
            return null;
        }
    }
}

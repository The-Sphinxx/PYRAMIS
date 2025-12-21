using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Agentic_Rentify.Application.Features.HomePage.DTOs;
using Agentic_Rentify.Application.Features.HomePage.Specifications;
using Agentic_Rentify.Application.Interfaces;
using Agentic_Rentify.Core.Entities;
using MediatR;

namespace Agentic_Rentify.Application.Features.HomePage.Queries.GetHomePageData;

public class GetHomePageDataQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetHomePageDataQuery, HomePageDataDto>
{
    public async Task<HomePageDataDto> Handle(GetHomePageDataQuery request, CancellationToken cancellationToken)
    {
        var trips = await unitOfWork.Repository<Trip>().ListAsync(new TopTripsSpecification(6));
        var attractions = await unitOfWork.Repository<Attraction>().ListAsync(new TopAttractionsSpecification(4));
        var hotels = await unitOfWork.Repository<Hotel>().ListAsync(new TopHotelsSpecification(4));
        var cars = await unitOfWork.Repository<Car>().ListAsync(new TopCarsSpecification(4));

        var featuredTrips = trips.Select(MapTrip).ToList();
        var popularAttractions = attractions.Select(MapAttraction).ToList();
        var featuredHotels = hotels.Select(MapHotel).ToList();
        var featuredCars = cars.Select(MapCar).ToList();

        // Resolve system background for Home page if configured
        var heroBackground = await ResolveHomeBackgroundAsync(unitOfWork, featuredTrips);

        return new HomePageDataDto
        {
            HeroSection = BuildHeroSection(featuredTrips, heroBackground),
            Metadata = BuildMetadata(),
            FeaturedTrips = featuredTrips,
            PopularAttractions = popularAttractions,
            FeaturedHotels = featuredHotels,
            FeaturedCars = featuredCars,
            Testimonials = BuildTestimonials(),
            WhyChooseUs = BuildWhyChooseUs()
        };
    }

    private static async Task<string> ResolveHomeBackgroundAsync(IUnitOfWork unitOfWork, IReadOnlyCollection<HomeTripCardDto> trips)
    {
        try
        {
            var spec = new Agentic_Rentify.Application.Features.SystemSettings.Specifications.SystemSettingByPageSpecification(Agentic_Rentify.Core.Enums.SystemPage.HomeHero);
            var settings = await unitOfWork.Repository<SystemSetting>().ListAsync(spec);
            var setting = settings.FirstOrDefault();
            if (setting is not null && !string.IsNullOrWhiteSpace(setting.ImageUrl))
            {
                return setting.ImageUrl;
            }
        }
        catch
        {
            // ignore and fallback
        }

        return trips.FirstOrDefault()?.Image ?? string.Empty;
    }

    private static HeroSectionDto BuildHeroSection(IReadOnlyCollection<HomeTripCardDto> trips, string? overrideBackground)
    {
        var firstImage = string.IsNullOrWhiteSpace(overrideBackground) ? trips.FirstOrDefault()?.Image : overrideBackground;

        return new HeroSectionDto
        {
            Title = "Discover Egypt — Your Journey Starts Here",
            Subtitle = "Explore ancient wonders, luxury stays, and unforgettable experiences",
            BackgroundImage = string.IsNullOrWhiteSpace(firstImage)
                ? "https://images.unsplash.com/photo-1568322445389-f64ac2515020?q=80&w=2070"
                : firstImage
        };
    }

    private static HomeMetadataDto BuildMetadata() => new()
    {
        SiteName = "PYRAMIS",
        PrimaryCtaText = "View All",
        SecondaryCtaText = "Start AI Planning",
        SupportEmail = "Pyramis@Pyramis.Com",
        SupportPhone = "+20 (123) 456-7890",
        Country = "Egypt"
    };

    private static HomeTripCardDto MapTrip(Trip trip)
    {
        return new HomeTripCardDto
        {
            Id = trip.Id,
            Name = trip.Title,
            Price = trip.Price,
            Description = trip.Description,
            Location = trip.City,
            Duration = trip.Duration,
            Rating = trip.Rating,
            Reviews = trip.TotalReviews.ToString("N0", CultureInfo.InvariantCulture),
            Image = ResolveImage(trip.MainImage, trip.Images)
        };
    }

    private static HomeAttractionCardDto MapAttraction(Attraction attraction)
    {
        var totalReviews = attraction.ReviewSummary?.TotalReviews ?? 0;

        return new HomeAttractionCardDto
        {
            Id = attraction.Id,
            Name = attraction.Name,
            Price = attraction.Price,
            Location = attraction.City,
            Rating = attraction.Rating,
            Reviews = totalReviews.ToString("N0", CultureInfo.InvariantCulture),
            Featured = attraction.Rating >= 4.5,
            Image = attraction.Images.FirstOrDefault()?.Url ?? string.Empty
        };
    }

    private static HomeHotelCardDto MapHotel(Hotel hotel)
    {
        return new HomeHotelCardDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Price = hotel.BasePrice,
            Location = hotel.City,
            Rating = hotel.Rating,
            Reviews = hotel.ReviewsCount.ToString("N0", CultureInfo.InvariantCulture),
            Image = hotel.Images.FirstOrDefault() ?? string.Empty
        };
    }

    private static HomeCarCardDto MapCar(Car car)
    {
        return new HomeCarCardDto
        {
            Id = car.Id,
            Name = car.Name,
            Price = car.Price,
            Description = car.Description,
            Image = car.Images.FirstOrDefault() ?? string.Empty
        };
    }

    private static List<HomeTestimonialDto> BuildTestimonials() => new()
    {
        new HomeTestimonialDto
        {
            Id = 1,
            Name = "Sarah Mitchell",
            Country = "United States",
            Text = "An unforgettable journey through ancient wonders. The pyramids exceeded every expectation, and our guide's knowledge brought history to life in the most magical way."
        },
        new HomeTestimonialDto
        {
            Id = 2,
            Name = "James Thompson",
            Country = "United Kingdom",
            Text = "Luxurious accommodations, impeccable service, and breathtaking sites at every turn. Egypt's beauty and rich culture left us absolutely mesmerized."
        },
        new HomeTestimonialDto
        {
            Id = 3,
            Name = "Marie Laurent",
            Country = "France",
            Text = "From the Nile cruise to the temples of Luxor, every moment was pure perfection. This trip transformed the way we see the world and ancient civilizations."
        }
    };

    private static List<WhyChooseUsDto> BuildWhyChooseUs() => new()
    {
        new WhyChooseUsDto { Id = 1, Icon = "fas fa-map", Title = "Complete Trips", Description = "Plan, book, and manage in one flow" },
        new WhyChooseUsDto { Id = 2, Icon = "fas fa-route", Title = "Built for Egypt", Description = "Local travel logic, not global templates." },
        new WhyChooseUsDto { Id = 3, Icon = "fas fa-plane-departure", Title = "AI Trip Planning", Description = "Personalized itineraries, real data." },
        new WhyChooseUsDto { Id = 4, Icon = "fas fa-shield-alt", Title = "Clear & Flexible", Description = "No hidden fees, full control." }
    };

    private static string ResolveImage(string primary, IEnumerable<string> alternatives)
    {
        if (!string.IsNullOrWhiteSpace(primary))
        {
            return primary;
        }

        return alternatives?.FirstOrDefault() ?? string.Empty;
    }
}

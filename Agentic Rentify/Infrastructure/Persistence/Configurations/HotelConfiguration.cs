// Infrastructure -> Persistence -> Configurations -> HotelConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.Property(h => h.Name).IsRequired().HasMaxLength(200);
        builder.Property(h => h.BasePrice).HasPrecision(18, 2);
        builder.Property(h => h.PricePerNight).HasPrecision(18, 2);

        // تخزين الغرف كـ JSON
        builder.OwnsMany(h => h.Rooms, r => 
        { 
            r.ToJson(); 
            r.Property(room => room.Price).HasPrecision(18, 2);
        });

        // تخزين الـ Review Summary كـ JSON
        builder.OwnsOne(h => h.ReviewSummary, rs =>
        {
            rs.ToJson();
            rs.OwnsOne(r => r.RatingCriteria);
        });

        // تخزين User Reviews كـ JSON
        builder.OwnsMany(h => h.UserReviews, ur => { ur.ToJson(); });

        // تخزين Availability كـ JSON
        builder.OwnsOne(h => h.Availability, av => { av.ToJson(); });

        // حقول الـ List<string> البسيطة
        builder.Property(h => h.Images).HasColumnType("nvarchar(max)");
        builder.Property(h => h.Amenities).HasColumnType("nvarchar(max)");
        builder.Property(h => h.RoomAmenities).HasColumnType("nvarchar(max)");
        builder.Property(h => h.Highlights).HasColumnType("nvarchar(max)");
        builder.Property(h => h.NearbyLocations).HasColumnType("nvarchar(max)");
    }
}
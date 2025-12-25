// Infrastructure -> Persistence -> Configurations -> TripConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.ToTable("Trips");

        builder.Property(t => t.Title).IsRequired().HasMaxLength(250);
        builder.Property(t => t.City).HasMaxLength(150);
        builder.Property(t => t.Destination).HasMaxLength(150);
        builder.Property(t => t.TripType).HasMaxLength(100);
        builder.Property(t => t.Price).HasPrecision(18, 2);
        builder.Property(t => t.StartDate);

        // تخزين الـ Itinerary كـ JSON (Nested List)
        builder.OwnsMany(t => t.Itinerary, i =>
        {
            i.ToJson();
            i.OwnsMany(d => d.Activities); // الـ Activities جوه الـ Day
        });

        // تخزين معلومات الفندق المرتبط بالرحلة كـ JSON
        builder.OwnsOne(t => t.HotelInfo, h => { h.ToJson(); });

        // تخزين الـ Review Summary كـ JSON
        builder.OwnsOne(t => t.ReviewSummary, rs =>
        {
            rs.ToJson();
            rs.OwnsOne(r => r.RatingCriteria);
        });

        // تخزين User Reviews كـ JSON
        builder.OwnsMany(t => t.UserReviews, ur => { ur.ToJson(); });

        // تخزين Amenities Info كـ JSON
        builder.OwnsOne(t => t.Amenities, ai => { ai.ToJson(); });

        // حقول الـ List<string> البسيطة
        builder.Property(t => t.AvailableDates).HasColumnType("nvarchar(max)");
        builder.Property(t => t.Images).HasColumnType("nvarchar(max)");
        builder.Property(t => t.Highlights).HasColumnType("nvarchar(max)");
        builder.Property(t => t.Reviews).HasColumnType("int");
    }
}
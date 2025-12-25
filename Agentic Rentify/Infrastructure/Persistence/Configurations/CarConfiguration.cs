// Infrastructure -> Persistence -> Configurations -> CarConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Brand).HasMaxLength(100);
        builder.Property(c => c.Model).HasMaxLength(100);
        builder.Property(c => c.Price).HasPrecision(18, 2);
        builder.Property(c => c.BasePrice).HasPrecision(18, 2);

        // تخزين الـ Review Summary كـ JSON
        builder.OwnsOne(c => c.ReviewSummary, rs =>
        {
            rs.ToJson();
            rs.OwnsOne(r => r.RatingCriteria);
        });

        // تخزين User Reviews كـ JSON
        builder.OwnsMany(c => c.UserReviews, ur => { ur.ToJson(); });

        // حقول الـ List<string> البسيطة
        builder.Property(c => c.Features).HasColumnType("nvarchar(max)");
        builder.Property(c => c.Images).HasColumnType("nvarchar(max)");
        builder.Property(c => c.Amenities).HasColumnType("nvarchar(max)");
    }
}
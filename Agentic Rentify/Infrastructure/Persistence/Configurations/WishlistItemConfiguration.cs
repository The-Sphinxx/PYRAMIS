using Agentic_Rentify.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agentic_Rentify.Infrastructure.Persistence.Configurations;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable("WishlistItems");

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ItemType)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(256);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(512);

        builder.Property(x => x.Location)
            .HasMaxLength(256);

        builder.HasIndex(x => new { x.UserId, x.ItemId, x.ItemType }).IsUnique();
    }
}

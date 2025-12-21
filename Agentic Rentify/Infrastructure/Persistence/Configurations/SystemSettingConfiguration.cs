using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PageName)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.ImageUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(s => s.PublicId)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(s => s.Group)
            .HasMaxLength(128);

        builder.Property(s => s.DisplayOrder)
            .HasColumnType("int");
    }
}

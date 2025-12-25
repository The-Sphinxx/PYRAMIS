using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(m => m.SenderId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.SenderName)
            .HasMaxLength(150);

        builder.Property(m => m.SenderRole)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ChatRole>(v))
            .HasMaxLength(20);

        builder.Property(m => m.SentAt).IsRequired();

        builder.HasIndex(m => m.ConversationId);
    }
}

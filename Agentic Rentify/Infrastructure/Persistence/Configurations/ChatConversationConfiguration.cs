using Agentic_Rentify.Core.Entities;
using Agentic_Rentify.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.ToTable("ChatConversations");

        builder.Property(c => c.Subject).HasMaxLength(200);
        builder.Property(c => c.ClientId).IsRequired().HasMaxLength(450);
        builder.Property(c => c.ClientName).HasMaxLength(150);
        builder.Property(c => c.ClientEmail).HasMaxLength(200);
        builder.Property(c => c.AssignedAdminId).HasMaxLength(450);
        builder.Property(c => c.AssignedAdminName).HasMaxLength(150);
        builder.Property(c => c.LastMessageAt).IsRequired();

        builder.Property(c => c.Status)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ChatStatus>(v))
            .HasMaxLength(20);

        builder.Property(c => c.Priority)
            .HasConversion(v => v.ToString(), v => Enum.Parse<ChatPriority>(v))
            .HasMaxLength(20);

        builder.HasMany(c => c.Messages)
            .WithOne(m => m.Conversation)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.ClientId);
        builder.HasIndex(c => c.LastMessageAt);
    }
}

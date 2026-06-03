using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class PostQueueItemConfiguration : IEntityTypeConfiguration<PostQueueItem>
{
    public void Configure(EntityTypeBuilder<PostQueueItem> builder)
    {
        builder.ToTable("post_queue_items");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Post).WithMany().HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ProfileId, x.Status, x.SortOrder });
    }
}

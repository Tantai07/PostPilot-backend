using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class PostHistoryConfiguration : IEntityTypeConfiguration<PostHistory>
{
    public void Configure(EntityTypeBuilder<PostHistory> builder)
    {
        builder.ToTable("post_history");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.Platform).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.ExternalPostId).HasMaxLength(256);
        builder.Property(x => x.Status).HasMaxLength(64).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(2048);
        builder.HasOne(x => x.Post).WithMany().HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PostId, x.Platform, x.CreatedAt });
    }
}

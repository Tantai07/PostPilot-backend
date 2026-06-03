using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class PostTargetConfiguration : IEntityTypeConfiguration<PostTarget>
{
    public void Configure(EntityTypeBuilder<PostTarget> builder)
    {
        builder.ToTable("post_targets");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.TargetPlatform).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.TargetAccountId).IsRequired();
        builder.HasOne(x => x.Post).WithMany(x => x.Targets).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PostId, x.TargetPlatform });
    }
}

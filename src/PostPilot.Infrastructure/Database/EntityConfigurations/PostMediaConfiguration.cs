using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ToTable("post_media");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.StorageProvider).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.PublicUrl).HasMaxLength(2048);
        builder.Property(x => x.SortOrder).IsRequired();
        builder.HasOne(x => x.Post).WithMany(x => x.Media).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.PostId, x.SortOrder });
    }
}

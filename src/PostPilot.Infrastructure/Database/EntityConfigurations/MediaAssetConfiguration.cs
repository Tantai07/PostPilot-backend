using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        builder.ToTable("media_assets");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.StorageProvider).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.PublicUrl).HasMaxLength(2048).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.MimeType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.SizeBytes).IsRequired();
        builder.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ProfileId, x.CreatedAt });
    }
}
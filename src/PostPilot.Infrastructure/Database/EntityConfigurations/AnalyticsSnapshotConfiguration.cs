using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class AnalyticsSnapshotConfiguration : IEntityTypeConfiguration<AnalyticsSnapshot>
{
    public void Configure(EntityTypeBuilder<AnalyticsSnapshot> builder)
    {
        builder.ToTable("analytics_snapshots");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.Platform).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.MetricName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.MetricValue).IsRequired();
        builder.Property(x => x.CapturedAt).IsRequired();
        builder.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ProfileId, x.Platform, x.MetricName, x.CapturedAt });
    }
}

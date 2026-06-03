using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("profiles");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.OwnerUserId).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(160).IsRequired();
        builder.Property(x => x.WebsiteName).HasMaxLength(160);
        builder.Property(x => x.DefaultTargets).HasMaxLength(512);
        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => x.Name);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class SocialAccountConfiguration : IEntityTypeConfiguration<SocialAccount>
{
    public void Configure(EntityTypeBuilder<SocialAccount> builder)
    {
        builder.ToTable("social_accounts");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.Platform).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.PageId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.IgUserId).HasMaxLength(128);
        builder.Property(x => x.DisplayName).HasMaxLength(160).IsRequired();
        builder.HasOne(x => x.Profile).WithMany(x => x.SocialAccounts).HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ProfileId, x.Platform });
    }
}

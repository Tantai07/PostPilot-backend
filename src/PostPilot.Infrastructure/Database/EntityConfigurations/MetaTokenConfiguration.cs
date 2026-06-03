using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class MetaTokenConfiguration : IEntityTypeConfiguration<MetaToken>
{
    public void Configure(EntityTypeBuilder<MetaToken> builder)
    {
        builder.ToTable("meta_tokens");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.EncryptedAccessToken).HasMaxLength(4096).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.HasOne(x => x.SocialAccount).WithOne(x => x.MetaToken).HasForeignKey<MetaToken>(x => x.SocialAccountId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.SocialAccountId).IsUnique();
    }
}

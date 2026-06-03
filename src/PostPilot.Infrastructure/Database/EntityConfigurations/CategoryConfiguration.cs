using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasOne(x => x.Profile).WithMany(x => x.Categories).HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.ProfileId, x.Name });
    }
}

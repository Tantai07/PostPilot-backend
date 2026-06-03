using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Entities;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

public sealed class CategoryTagConfiguration : IEntityTypeConfiguration<CategoryTag>
{
    public void Configure(EntityTypeBuilder<CategoryTag> builder)
    {
        builder.ToTable("category_tags");
        builder.ConfigureSoftDeleteEntity();
        builder.Property(x => x.TagText).HasMaxLength(120).IsRequired();
        builder.Property(x => x.SortOrder).IsRequired();
        builder.HasOne(x => x.Category).WithMany(x => x.Tags).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CategoryId, x.SortOrder });
    }
}

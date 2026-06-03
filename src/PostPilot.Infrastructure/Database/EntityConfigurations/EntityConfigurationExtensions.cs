using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostPilot.Domain.Common;

namespace PostPilot.Infrastructure.Database.EntityConfigurations;

internal static class EntityConfigurationExtensions
{
    public static void ConfigureSoftDeleteEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : SoftDeleteEntity
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.CreatedAt);
    }
}

using Microsoft.EntityFrameworkCore;
using PostPilot.Domain.Common;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Auth;

namespace PostPilot.Infrastructure.Database;

public sealed class AppDbContext : DbContext
{
    private readonly ICurrentUserContext? _currentUserContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserContext? currentUserContext = null)
        : base(options)
    {
        _currentUserContext = currentUserContext;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<SocialAccount> SocialAccounts => Set<SocialAccount>();
    public DbSet<MetaToken> MetaTokens => Set<MetaToken>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CategoryTag> CategoryTags => Set<CategoryTag>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostMedia> PostMedia => Set<PostMedia>();
    public DbSet<PostTarget> PostTargets => Set<PostTarget>();
    public DbSet<PostQueueItem> PostQueueItems => Set<PostQueueItem>();
    public DbSet<PostHistory> PostHistory => Set<PostHistory>();
    public DbSet<AnalyticsSnapshot> AnalyticsSnapshots => Set<AnalyticsSnapshot>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    private void ApplyAuditFields()
    {
        var now = DateTimeOffset.UtcNow;
        var userId = _currentUserContext?.UserId;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy ??= userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = userId;
            }
        }
    }
}

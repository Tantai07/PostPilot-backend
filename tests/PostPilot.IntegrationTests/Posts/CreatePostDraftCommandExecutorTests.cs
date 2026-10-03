using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Posts.Commands;
using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Posts;

public sealed class CreatePostDraftCommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_PreservesAllSelectedChannelsAndMediaOrder()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AppDbContext(options);
        var owner = Guid.NewGuid();
        var profile = new Profile(owner, "Shop", "Shop", "Facebook,Instagram,X,TikTok,TikTok Shop");
        var first = new MediaAsset(profile.Id, StorageProvider.Local, "/first.mp4", "/first.mp4", "first.mp4", "video/mp4", 20);
        var second = new MediaAsset(profile.Id, StorageProvider.Local, "/second.jpg", "/second.jpg", "second.jpg", "image/jpeg", 10);
        db.Profiles.Add(profile);
        db.MediaAssets.AddRange(first, second);
        await db.SaveChangesAsync();
        var result = await new CreatePostDraftCommandExecutor(db).ExecuteAsync(owner, profile.Id, new CreatePostDraftRequestDto
        {
            Caption = "Sale #shop",
            MediaIds = [second.Id, first.Id],
            TargetPlatforms = ["Facebook Page", "Facebook Story", "Instagram Feed", "Instagram Story", "X", "TikTok", "TikTok Story", "TikTok Shop"]
        }, CancellationToken.None);
        result.Should().NotBeNull();
        result!.TargetPlatforms.Should().HaveCount(8);
        var media = await db.PostMedia.Where(x => x.PostId == result.Id).OrderBy(x => x.SortOrder).ToListAsync();
        media.Select(x => x.PublicUrl).Should().Equal("/second.jpg", "/first.mp4");
    }
}

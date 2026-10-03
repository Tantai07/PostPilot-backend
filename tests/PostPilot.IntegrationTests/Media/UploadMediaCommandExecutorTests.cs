using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Media.Commands;
using PostPilot.Api.Features.Media.Storage;
using PostPilot.Domain.Entities;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.IntegrationTests.Media;

public sealed class UploadMediaCommandExecutorTests
{
    [Theory]
    [InlineData("video/mp4", 100 * 1024 * 1024, true)]
    [InlineData("video/webm", 12 * 1024 * 1024, true)]
    [InlineData("video/quicktime", 12 * 1024 * 1024, true)]
    [InlineData("video/mp4", 100 * 1024 * 1024 + 1, false)]
    [InlineData("image/jpeg", 10 * 1024 * 1024 + 1, false)]
    [InlineData("application/octet-stream", 10, false)]
    public async Task ExecuteAsync_AppliesMediaTypeAndSizeLimits(string mime, long length, bool accepted)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new AppDbContext(options);
        var owner = Guid.NewGuid();
        var profile = new Profile(owner, "Shop", "Shop", "TikTok");
        db.Profiles.Add(profile);
        await db.SaveChangesAsync();
        var storage = new RecordingStorage();
        var executor = new UploadMediaCommandExecutor(db, storage);
        using var stream = new MemoryStream([1]);
        var file = new FormFile(stream, 0, length, "file", "sample") { Headers = new HeaderDictionary(), ContentType = mime };
        var action = () => executor.ExecuteAsync(owner, profile.Id, file, new DefaultHttpContext().Request, CancellationToken.None);
        if (accepted)
        {
            var result = await action();
            result.Should().NotBeNull();
            storage.Called.Should().BeTrue();
            (await db.MediaAssets.SingleAsync()).MimeType.Should().Be(mime);
        }
        else
        {
            await action.Should().ThrowAsync<InvalidOperationException>();
            storage.Called.Should().BeFalse();
            (await db.MediaAssets.CountAsync()).Should().Be(0);
        }
    }

    private sealed class RecordingStorage : IMediaStorageService
    {
        public bool Called { get; private set; }
        public Task<MediaStorageResult> UploadAsync(Guid profileId, IFormFile file, HttpRequest request, CancellationToken cancellationToken)
        {
            Called = true;
            return Task.FromResult(new MediaStorageResult(StorageProvider.Local, "/sample", "/sample", file.FileName, file.ContentType, file.Length));
        }
    }
}

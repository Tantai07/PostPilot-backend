using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostPilot.Api.Features.Meta.Security;
using PostPilot.Domain.Enums;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Publishing.Meta;

public sealed class MetaPostPublisher : IPostPublisher
{
    private readonly AppDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly MetaCredentialCodec _codec;
    private readonly PublishingOptions _options;

    public MetaPostPublisher(
        AppDbContext dbContext,
        HttpClient httpClient,
        MetaCredentialCodec codec,
        IOptions<PublishingOptions> options)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _codec = codec;
        _options = options.Value;
    }

    public async Task<PublishResult> PublishAsync(
        Guid postId,
        PostTargetPlatform platform,
        CancellationToken cancellationToken)
    {
        if (platform != PostTargetPlatform.FacebookPage)
        {
            return new PublishResult(platform, false, null, "Meta publisher currently supports Facebook Page only.");
        }

        var post = await _dbContext.Posts
            .AsNoTracking()
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == postId && !x.IsDeleted, cancellationToken);

        if (post is null)
        {
            return new PublishResult(platform, false, null, "Post was not found.");
        }

        var media = post.Media
            .Where(x => !x.IsDeleted && !string.IsNullOrWhiteSpace(x.PublicUrl))
            .OrderBy(x => x.SortOrder)
            .FirstOrDefault();

        if (media is null)
        {
            return new PublishResult(platform, false, null, "Facebook Page publish requires at least one uploaded public image URL.");
        }

        var account = await _dbContext.SocialAccounts
            .AsNoTracking()
            .Include(x => x.MetaToken)
            .FirstOrDefaultAsync(x =>
                x.ProfileId == post.ProfileId
                && x.Platform == SocialPlatform.Facebook
                && !x.IsDeleted,
                cancellationToken);

        if (account?.MetaToken is null || account.MetaToken.IsDeleted)
        {
            return new PublishResult(platform, false, null, "Facebook Page connection is not configured.");
        }

        if (account.MetaToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return new PublishResult(platform, false, null, "Facebook Page credential is expired.");
        }

        var credential = _codec.Decode(account.MetaToken.EncryptedAccessToken);
        return await PublishPhotoAsync(account.PageId, credential, post.Caption, media.PublicUrl!, platform, cancellationToken);
    }

    private async Task<PublishResult> PublishPhotoAsync(
        string pageId,
        string credential,
        string caption,
        string publicImageUrl,
        PostTargetPlatform platform,
        CancellationToken cancellationToken)
    {
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["url"] = publicImageUrl,
            ["caption"] = caption,
            ["access_token"] = credential
        });

        var apiVersion = string.IsNullOrWhiteSpace(_options.GraphApiVersion) ? "v20.0" : _options.GraphApiVersion.Trim();
        var endpoint = $"https://graph.facebook.com/{apiVersion}/{pageId}/photos";
        using var response = await _httpClient.PostAsync(endpoint, form, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new PublishResult(platform, false, null, ExtractErrorMessage(body));
        }

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        var externalPostId = root.TryGetProperty("post_id", out var postIdElement)
            ? postIdElement.GetString()
            : root.TryGetProperty("id", out var idElement)
                ? idElement.GetString()
                : null;

        return new PublishResult(platform, true, externalPostId, null);
    }

    private static string ExtractErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return "Meta API returned an empty error response.";
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;
            if (root.TryGetProperty("error", out var error)
                && error.TryGetProperty("message", out var message))
            {
                return message.GetString() ?? "Meta API returned an error.";
            }
        }
        catch (JsonException)
        {
            return responseBody;
        }

        return responseBody;
    }
}

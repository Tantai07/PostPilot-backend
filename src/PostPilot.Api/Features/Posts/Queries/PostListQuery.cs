using PostPilot.Api.Shared;

namespace PostPilot.Api.Features.Posts.Queries;

public sealed class PostListQuery : BaseQuery
{
    public string? Status { get; set; }
}

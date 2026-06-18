namespace PostPilot.Api.Features.Queue.Dtos;

public sealed class AddPostToQueueRequestDto
{
    public DateTimeOffset? ScheduledAt { get; set; }
}
namespace PostPilot.Api.Features.Queue.Dtos;

public sealed class ReorderQueueRequestDto
{
    public List<Guid> QueueItemIds { get; set; } = [];
}
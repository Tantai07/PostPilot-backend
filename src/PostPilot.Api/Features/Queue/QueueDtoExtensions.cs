using PostPilot.Api.Features.Queue.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Queue;

public static class QueueDtoExtensions
{
    public static QueueItemResponseDto ToDto(this PostQueueItem entity)
        => QueueItemResponseDto.FromEntity(entity);
}
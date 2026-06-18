using PostPilot.Api.Features.History.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.History;

public static class HistoryDtoExtensions
{
    public static PostHistoryResponseDto ToDto(this PostHistory entity)
        => PostHistoryResponseDto.FromEntity(entity);
}
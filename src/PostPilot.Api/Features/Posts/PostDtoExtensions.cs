using PostPilot.Api.Features.Posts.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Posts;

public static class PostDtoExtensions
{
    public static PostResponseDto ToDto(this Post entity)
        => PostResponseDto.FromEntity(entity);
}
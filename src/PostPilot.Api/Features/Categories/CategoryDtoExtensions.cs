using PostPilot.Api.Features.Categories.Dtos;
using PostPilot.Domain.Entities;

namespace PostPilot.Api.Features.Categories;

public static class CategoryDtoExtensions
{
    public static CategoryResponseDto ToDto(this Category entity)
        => CategoryResponseDto.FromEntity(entity);
}
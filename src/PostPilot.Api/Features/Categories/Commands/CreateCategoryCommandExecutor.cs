using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Categories.Dtos;
using PostPilot.Domain.Entities;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Categories.Commands;

public sealed class CreateCategoryCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public CreateCategoryCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        CreateCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var profileExists = await _dbContext.Profiles
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId, cancellationToken);

        if (!profileExists)
        {
            return null;
        }

        var category = new Category(profileId, request.Name, request.Color, request.Description)
        {
            CreatedBy = ownerUserId
        };

        category.ReplaceTags(CategoryTagInputMapper.ToTagInputs(request.Tags), ownerUserId, DateTimeOffset.UtcNow);

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}
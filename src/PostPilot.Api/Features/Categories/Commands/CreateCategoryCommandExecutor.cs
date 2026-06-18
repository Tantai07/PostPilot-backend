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
        CategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var canUseProfile = await _dbContext.Profiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == profileId && x.OwnerUserId == ownerUserId && !x.IsDeleted, cancellationToken);

        if (!canUseProfile)
        {
            return null;
        }

        var category = new Category(
            profileId,
            request.Name,
            request.Color,
            request.Description,
            request.CaptionTemplate,
            request.Tags)
        {
            CreatedBy = ownerUserId
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}

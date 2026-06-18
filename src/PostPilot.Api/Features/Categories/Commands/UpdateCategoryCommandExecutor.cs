using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Categories.Dtos;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Categories.Commands;

public sealed class UpdateCategoryCommandExecutor
{
    private readonly AppDbContext _dbContext;

    public UpdateCategoryCommandExecutor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryResponseDto?> ExecuteAsync(
        Guid ownerUserId,
        Guid profileId,
        Guid categoryId,
        CategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .Include(x => x.Profile)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x =>
                x.Id == categoryId
                && x.ProfileId == profileId
                && x.Profile != null
                && x.Profile.OwnerUserId == ownerUserId,
                cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Rename(request.Name, request.Color, request.Description, request.CaptionTemplate);
        category.ReplaceTags(request.Tags);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return category.ToDto();
    }
}

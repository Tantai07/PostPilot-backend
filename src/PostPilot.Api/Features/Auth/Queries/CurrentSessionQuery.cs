using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Auth.Queries;

public sealed class CurrentSessionQuery
{
    private readonly AppDbContext _dbContext;

    public CurrentSessionQuery(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto?> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive, cancellationToken);

        return user?.ToDto();
    }
}

using PostPilot.Domain.Common;

namespace PostPilot.Api.Shared;

public abstract class ResponseDtoBase<TEntity>
    where TEntity : BaseEntity
{
    protected ResponseDtoBase(TEntity entity)
    {
        Id = entity.Id;
    }

    public Guid Id { get; init; }
}

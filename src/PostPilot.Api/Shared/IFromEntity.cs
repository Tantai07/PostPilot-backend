namespace PostPilot.Api.Shared;

public interface IFromEntity<TEntity, TDto>
{
    static abstract TDto FromEntity(TEntity entity);
}

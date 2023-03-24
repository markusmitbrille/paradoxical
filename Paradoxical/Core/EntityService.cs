using Paradoxical.Services;
using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IEntityService<T>
    where T : IEntity
{
    T Get(int id);
    T Get(T entity);

    IEnumerable<T> Get();

    void Insert(T entity);
    void Delete(T entity);

    void Update(T entity);
    void UpdateAll(IEnumerable<T> entity);
}

public abstract class EntityService<T> : IEntityService<T>
    where T : IEntity, new()
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    public EntityService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public T Get(int id)
    {
        return Data.Connection.Get<T>(id);
    }
    public T Get(T entity)
    {
        return Get(entity.Id);
    }

    public IEnumerable<T> Get()
    {
        return Data.Connection.Table<T>().ToArray();
    }

    public void Insert(T entity)
    {
        Data.Connection.Insert(entity);
    }
    public void Delete(T entity)
    {
        Data.Connection.Delete(entity);
    }

    public void Update(T entity)
    {
        Data.Connection.Update(entity);
    }
    public void UpdateAll(IEnumerable<T> entity)
    {
        Data.Connection.UpdateAll(entity);
    }
}

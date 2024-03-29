﻿using Paradoxical.Services;
using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IEntityService<T>
    where T : IEntity
{
    T Get(int id);
    T Get(T entity);

    IEnumerable<T> Get();

    T? Find(int id);
    T? Find(T entity);

    void Insert(T entity);
    void Delete(T entity);

    void Update(T entity);
    void UpdateAll(IEnumerable<T> entity);

    T Clone(T entity);
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

    public virtual T Get(int id)
    {
        return Data.Connection.Get<T>(id);
    }
    public virtual T Get(T entity)
    {
        return Get(entity.Id);
    }

    public virtual IEnumerable<T> Get()
    {
        return Data.Connection.Table<T>().ToArray();
    }

    public virtual T? Find(int id)
    {
        return Data.Connection.Find<T>(id);
    }
    public virtual T? Find(T entity)
    {
        return Find(entity.Id);
    }

    public virtual void Insert(T entity)
    {
        Data.Connection.Insert(entity);
    }
    public virtual void Delete(T entity)
    {
        Data.Connection.Delete(entity);
    }

    public virtual void Update(T entity)
    {
        Data.Connection.Update(entity);
    }
    public virtual void UpdateAll(IEnumerable<T> entity)
    {
        Data.Connection.UpdateAll(entity);
    }

    public T Clone(T entity)
    {
        T copy = Get(entity);
        Data.Connection.Insert(copy);

        return copy;
    }
}

using Paradoxical.Messages;
using Paradoxical.Services;
using System.Collections.Generic;

namespace Paradoxical.Core;

public interface IElementService<TElement> where TElement : IElement
{
    TElement Get(int id);
    TElement Get(TElement model);

    IEnumerable<TElement> Get();

    void Insert(TElement model);
    void Delete(TElement model);

    void Update(TElement model);
    void UpdateAll(IEnumerable<TElement> models);
}

public abstract class ElementService<TElement> : IElementService<TElement> where TElement : IElement, new()
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    public ElementService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public TElement Get(int id)
    {
        return Data.Connection.Get<TElement>(id);
    }
    public TElement Get(TElement model)
    {
        return Get(model.Id);
    }

    public IEnumerable<TElement> Get()
    {
        return Data.Connection.Table<TElement>().ToArray();
    }

    public void Insert(TElement model)
    {
        Data.Connection.Insert(model);

        Mediator.Send<ElementInsertedMessage>(new(model));
        Mediator.Send<ElementSelectedMessage>(new(model));
    }
    public void Delete(TElement model)
    {
        Data.Connection.Delete(model);

        Mediator.Send<ElementDeletedMessage>(new(model));
        Mediator.Send<ElementDeselectedMessage>(new(model));
    }

    public void Update(TElement model)
    {
        Data.Connection.Update(model);
    }
    public void UpdateAll(IEnumerable<TElement> models)
    {
        Data.Connection.UpdateAll(models);
    }
}

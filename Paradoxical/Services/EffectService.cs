using Paradoxical.Messages;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEffectService
{
    Effect Get(int id);
    Effect Get(Effect model);

    IEnumerable<Effect> Get();

    void Insert(Effect model);
    void Delete(Effect model);

    void Update(Effect model);
    void UpdateAll(IEnumerable<Effect> models);
}

public class EffectService : IEffectService
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    public EffectService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public Effect Get(int id)
    {
        return Data.Connection.Get<Effect>(id);
    }
    public Effect Get(Effect model)
    {
        return Get(model.Id);
    }

    public IEnumerable<Effect> Get()
    {
        return Data.Connection.Table<Effect>().ToArray();
    }

    public void Insert(Effect model)
    {
        Data.Connection.Insert(model);

        Mediator.Send<ElementInsertedMessage>(new(model));
        Mediator.Send<ElementSelectedMessage>(new(model));
    }
    public void Delete(Effect model)
    {
        Data.Connection.Delete(model);

        Mediator.Send<ElementDeletedMessage>(new(model));
        Mediator.Send<ElementDeselectedMessage>(new(model));
    }

    public void Update(Effect model)
    {
        Data.Connection.Update(model);
    }
    public void UpdateAll(IEnumerable<Effect> models)
    {
        Data.Connection.UpdateAll(models);
    }
}
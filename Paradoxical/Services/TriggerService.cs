using Paradoxical.Messages;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface ITriggerService
{
    Trigger Get(int id);
    Trigger Get(Trigger model);
    IEnumerable<Trigger> Get();

    void Insert(Trigger model);
    void Delete(Trigger model);

    void Update(Trigger model);
    void UpdateAll(IEnumerable<Trigger> models);
}

public class TriggerService : ITriggerService
{
    public IDataService Data { get; }
    public IMediatorService Mediator { get; }

    public TriggerService(
        IDataService data,
        IMediatorService mediator)
    {
        Data = data;
        Mediator = mediator;
    }

    public Trigger Get(int id)
    {
        return Data.Connection.Get<Trigger>(id);
    }
    public Trigger Get(Trigger model)
    {
        return Get(model.Id);
    }

    public IEnumerable<Trigger> Get()
    {
        return Data.Connection.Table<Trigger>().ToArray();
    }

    public void Insert(Trigger model)
    {
        Data.Connection.Insert(model);

        Mediator.Send<InsertMessage>(new(model));
        Mediator.Send<SelectMessage>(new(model));
    }
    public void Delete(Trigger model)
    {
        Data.Connection.Delete(model);

        Mediator.Send<DeleteMessage>(new(model));
        Mediator.Send<DeselectMessage>(new(model));
    }

    public void Update(Trigger model)
    {
        Data.Connection.Update(model);
    }
    public void UpdateAll(IEnumerable<Trigger> models)
    {
        Data.Connection.UpdateAll(models);
    }
}

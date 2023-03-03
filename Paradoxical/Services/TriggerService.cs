using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface ITriggerService
{
    IEnumerable<Trigger> Get();

    void Insert(Trigger model);
    void Delete(Trigger model);

    void Update(Trigger model);
    void UpdateAll(IEnumerable<Trigger> models);
}

public class TriggerService : ITriggerService
{
    public IDataService Data { get; }

    public TriggerService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Trigger> Get()
    {
        return Data.Connection.Table<Trigger>().ToArray();
    }

    public void Insert(Trigger model)
    {
        Data.Connection.Insert(model);
    }
    public void Delete(Trigger model)
    {
        Data.Connection.Delete(model);
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

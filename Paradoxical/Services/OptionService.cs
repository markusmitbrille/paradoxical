using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IOptionService
{
    IEnumerable<Option> Get();

    void Insert(Option model);
    void Update(Option model);
    void Delete(Option model);

    Event GetOwner(Option model);
    Event? GetTriggeredEvent(Option model);

    IEnumerable<Trigger> GetTriggers(Option model);
    void SetTriggers(Option model, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(Option model);
    void SetEffects(Option model, IEnumerable<Effect> relations);
}

public class OptionService : IOptionService
{
    public IDataService Data { get; }

    public OptionService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Option> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(Option model)
    {
        throw new NotImplementedException();
    }
    public void Update(Option model)
    {
        throw new NotImplementedException();
    }
    public void Delete(Option model)
    {
        throw new NotImplementedException();
    }

    public Event GetOwner(Option model)
    {
        throw new NotImplementedException();
    }
    public Event? GetTriggeredEvent(Option model)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetTriggers(Option model)
    {
        throw new NotImplementedException();
    }
    public void SetTriggers(Option model, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(Option model)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(Option model, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

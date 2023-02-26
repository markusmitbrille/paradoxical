using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEventService
{
    IEnumerable<Event> Get();

    void Insert(Event element);
    void Update(Event element);
    void Delete(Event element);

    IEnumerable<Trigger> GetTriggers(Event element);
    void SetTriggers(Event element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetImmediateEffects(Event element);
    void SetImmediateEffects(Event element, IEnumerable<Effect> relations);

    IEnumerable<Effect> GetAfterEffects(Event element);
    void SetAfterEffects(Event element, IEnumerable<Effect> relations);

    IEnumerable<Option> GetOptions(Event element);
    void SetOptions(Event element, IEnumerable<Effect> relations);
}

public class EventService : IEventService
{
    public IDataService Data { get; }

    public EventService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Event> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(Event element)
    {
        throw new NotImplementedException();
    }
    public void Update(Event element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Event element)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetTriggers(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetTriggers(Event element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetImmediateEffects(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetImmediateEffects(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetAfterEffects(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetAfterEffects(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Option> GetOptions(Event element)
    {
        throw new NotImplementedException();
    }
    public void SetOptions(Event element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

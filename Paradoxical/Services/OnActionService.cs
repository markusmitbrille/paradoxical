using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IOnActionService
{
    IEnumerable<OnAction> Get();

    void Insert(OnAction element);
    void Update(OnAction element);
    void Delete(OnAction element);

    IEnumerable<Trigger> GetTriggers(OnAction element);
    void SetTriggers(OnAction element, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(OnAction element);
    void SetEffects(OnAction element, IEnumerable<Effect> relations);

    IEnumerable<Event> GetEvents(OnAction element);
    void SetEvents(OnAction element, IEnumerable<Event> relations);

    IEnumerable<OnAction> GetOnActions(OnAction element);
    void SetOnActions(OnAction element, IEnumerable<OnAction> relations);
}

public class OnActionService : IOnActionService
{
    public IDataService Data { get; }

    public OnActionService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<OnAction> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void Update(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void Delete(OnAction element)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetTriggers(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void SetTriggers(OnAction element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(OnAction element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Event> GetEvents(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void SetEvents(OnAction element, IEnumerable<Event> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OnAction> GetOnActions(OnAction element)
    {
        throw new NotImplementedException();
    }
    public void SetOnActions(OnAction element, IEnumerable<OnAction> relations)
    {
        throw new NotImplementedException();
    }
}
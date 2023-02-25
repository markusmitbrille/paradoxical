using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

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
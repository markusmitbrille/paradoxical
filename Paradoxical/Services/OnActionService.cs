using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IOnActionService
{
    IEnumerable<OnAction> Get();

    void Insert(OnAction model);
    void Update(OnAction model);
    void Delete(OnAction model);

    IEnumerable<Trigger> GetTriggers(OnAction model);
    void SetTriggers(OnAction model, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(OnAction model);
    void SetEffects(OnAction model, IEnumerable<Effect> relations);

    IEnumerable<Event> GetEvents(OnAction model);
    void SetEvents(OnAction model, IEnumerable<Event> relations);

    IEnumerable<OnAction> GetOnActions(OnAction model);
    void SetOnActions(OnAction model, IEnumerable<OnAction> relations);
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

    public void Insert(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void Update(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void Delete(OnAction model)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetTriggers(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void SetTriggers(OnAction model, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(OnAction model, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Event> GetEvents(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void SetEvents(OnAction model, IEnumerable<Event> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<OnAction> GetOnActions(OnAction model)
    {
        throw new NotImplementedException();
    }
    public void SetOnActions(OnAction model, IEnumerable<OnAction> relations)
    {
        throw new NotImplementedException();
    }
}
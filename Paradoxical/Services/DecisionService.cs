using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IDecisionService
{
    IEnumerable<Decision> Get();

    void Insert(Decision model);
    void Update(Decision model);
    void Delete(Decision model);

    Event? GetTriggeredEvent(Decision model);

    IEnumerable<Trigger> GetIsShownTriggers(Decision model);
    void SetIsShownTriggers(Decision model, IEnumerable<Trigger> relations);

    IEnumerable<Trigger> GetIsValidTriggers(Decision model);
    void SetIsValidTriggers(Decision model, IEnumerable<Trigger> relations);

    IEnumerable<Trigger> GetIsValidFailureTriggers(Decision model);
    void SetIsValidFailureTriggers(Decision model, IEnumerable<Trigger> relations);

    IEnumerable<Effect> GetEffects(Decision model);
    void SetEffects(Decision model, IEnumerable<Effect> relations);
}

public class DecisionService : IDecisionService
{
    public IDataService Data { get; }

    public DecisionService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Decision> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(Decision model)
    {
        throw new NotImplementedException();
    }
    public void Update(Decision model)
    {
        throw new NotImplementedException();
    }
    public void Delete(Decision model)
    {
        throw new NotImplementedException();
    }

    public Event? GetTriggeredEvent(Decision model)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsShownTriggers(Decision model)
    {
        throw new NotImplementedException();
    }
    public void SetIsShownTriggers(Decision model, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsValidTriggers(Decision model)
    {
        throw new NotImplementedException();
    }
    public void SetIsValidTriggers(Decision model, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsValidFailureTriggers(Decision model)
    {
        throw new NotImplementedException();
    }
    public void SetIsValidFailureTriggers(Decision model, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(Decision model)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(Decision model, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

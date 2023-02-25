using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public class DecisionService : IDecisionService
{
    public DataService Data { get; }

    public DecisionService(DataService data)
    {
        Data = data;
    }

    public IEnumerable<Decision> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(Decision element)
    {
        throw new NotImplementedException();
    }
    public void Update(Decision element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Decision element)
    {
        throw new NotImplementedException();
    }

    public Event? GetTriggeredEvent(Decision element)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsShownTriggers(Decision element)
    {
        throw new NotImplementedException();
    }
    public void SetIsShownTriggers(Decision element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsValidTriggers(Decision element)
    {
        throw new NotImplementedException();
    }
    public void SetIsValidTriggers(Decision element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetIsValidFailureTriggers(Decision element)
    {
        throw new NotImplementedException();
    }
    public void SetIsValidFailureTriggers(Decision element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(Decision element)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(Decision element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

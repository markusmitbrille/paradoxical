using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

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

    public void Insert(Option element)
    {
        throw new NotImplementedException();
    }
    public void Update(Option element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Option element)
    {
        throw new NotImplementedException();
    }

    public Event GetOwner(Option element)
    {
        throw new NotImplementedException();
    }
    public Event? GetTriggeredEvent(Option element)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Trigger> GetTriggers(Option element)
    {
        throw new NotImplementedException();
    }
    public void SetTriggers(Option element, IEnumerable<Trigger> relations)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Effect> GetEffects(Option element)
    {
        throw new NotImplementedException();
    }
    public void SetEffects(Option element, IEnumerable<Effect> relations)
    {
        throw new NotImplementedException();
    }
}

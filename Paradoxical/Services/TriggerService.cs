using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface ITriggerService
{
    IEnumerable<Trigger> Get();

    void Insert(Trigger element);
    void Update(Trigger element);
    void Delete(Trigger element);
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
        throw new NotImplementedException();
    }

    public void Insert(Trigger element)
    {
        throw new NotImplementedException();
    }
    public void Update(Trigger element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Trigger element)
    {
        throw new NotImplementedException();
    }
}

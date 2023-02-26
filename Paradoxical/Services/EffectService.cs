using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEffectService
{
    IEnumerable<Effect> Get();

    void Insert(Effect element);
    void Update(Effect element);
    void Delete(Effect element);
}

public class EffectService : IEffectService
{
    public IDataService Data { get; }

    public EffectService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Effect> Get()
    {
        throw new NotImplementedException();
    }

    public void Insert(Effect element)
    {
        throw new NotImplementedException();
    }
    public void Update(Effect element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Effect element)
    {
        throw new NotImplementedException();
    }
}
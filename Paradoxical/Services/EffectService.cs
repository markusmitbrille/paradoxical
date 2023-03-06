using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IEffectService
{
    IEnumerable<Effect> Get();

    void Insert(Effect model);
    void Update(Effect model);
    void Delete(Effect model);
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

    public void Insert(Effect model)
    {
        throw new NotImplementedException();
    }
    public void Update(Effect model)
    {
        throw new NotImplementedException();
    }
    public void Delete(Effect model)
    {
        throw new NotImplementedException();
    }
}
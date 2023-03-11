using Paradoxical.Model.Components;
using Paradoxical.Model.Elements;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services.Components;

public interface IPortraitService
{
    IEnumerable<Portrait> Get();

    Portrait? Get(Event owner, PortraitPosition position);

    void Insert(Portrait model);
    void Update(Portrait model);
    void Delete(Portrait model);
}

public class PortraitService : IPortraitService
{
    public IDataService Data { get; }

    public PortraitService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<Portrait> Get()
    {
        throw new NotImplementedException();
    }

    public Portrait? Get(Event owner, PortraitPosition position)
    {
        throw new NotImplementedException();
    }

    public void Insert(Portrait model)
    {
        throw new NotImplementedException();
    }
    public void Update(Portrait model)
    {
        throw new NotImplementedException();
    }
    public void Delete(Portrait model)
    {
        throw new NotImplementedException();
    }
}

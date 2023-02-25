using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

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

    public void Insert(Portrait element)
    {
        throw new NotImplementedException();
    }
    public void Update(Portrait element)
    {
        throw new NotImplementedException();
    }
    public void Delete(Portrait element)
    {
        throw new NotImplementedException();
    }
}

using Paradoxical.Model;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public class ElementService : IElementService
{
    public IDataService Data { get; }

    public ElementService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<IElement> Get()
    {
        throw new NotImplementedException();
    }
}
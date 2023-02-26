using Paradoxical.Core;
using System;
using System.Collections.Generic;

namespace Paradoxical.Services;

public interface IElementService
{
    IEnumerable<IElementModel> Get();
}

public class ElementService : IElementService
{
    public IDataService Data { get; }

    public ElementService(IDataService data)
    {
        Data = data;
    }

    public IEnumerable<IElementModel> Get()
    {
        throw new NotImplementedException();
    }
}
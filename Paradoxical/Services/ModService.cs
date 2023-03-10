using Paradoxical.Model;
using System;

namespace Paradoxical.Services;

public interface IModService
{
    Mod Get();

    string GetPrefix();
    string GetModName();
}

public class ModService : IModService
{
    public IDataService Data { get; }

    public ModService(IDataService data)
    {
        Data = data;
    }

    public Mod Get()
    {
        throw new NotImplementedException();
    }

    public string GetPrefix()
    {
        throw new NotImplementedException();
    }
    public string GetModName()
    {
        throw new NotImplementedException();
    }
}

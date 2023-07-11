using Paradoxical.Core;
using Paradoxical.Model;
using System;
using System.Linq;

namespace Paradoxical.Services.Entities;

public interface IModService : IEntityService<Mod>
{
    public string GetModName();
    public string GetModVersion();
    public string GetGameVersion();
    public string GetPrefix();
}

public class ModService : EntityService<Mod>, IModService
{
    public ModService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }

    public string GetModName()
    {
        var mod = Get().SingleOrDefault() ?? new();
        return mod.ModName;
    }

    public string GetModVersion()
    {
        var mod = Get().SingleOrDefault() ?? new();
        return mod.ModVersion;
    }

    public string GetGameVersion()
    {
        var mod = Get().SingleOrDefault() ?? new();
        return mod.GameVersion;
    }

    public string GetPrefix()
    {
        var mod = Get().SingleOrDefault() ?? new();
        return mod.Prefix;
    }
}

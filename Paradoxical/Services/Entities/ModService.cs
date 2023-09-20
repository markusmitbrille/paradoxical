using Paradoxical.Core;
using Paradoxical.Model.Entities;
using System;
using System.Linq;

namespace Paradoxical.Services.Entities;

public interface IModService : IEntityService<Mod>
{
    public Mod GetOne();

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

    public Mod GetOne()
    {
        var entity = Get().SingleOrDefault();
        if (entity == null)
        {
            entity = new();
            Insert(entity);
        }

        return entity;
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

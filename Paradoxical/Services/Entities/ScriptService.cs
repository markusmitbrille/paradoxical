using Paradoxical.Core;
using Paradoxical.Model.Entities;

namespace Paradoxical.Services.Entities;

public interface IScriptService : IEntityService<Script>
{
}

public class ScriptService : EntityService<Script>, IScriptService
{
    public ScriptService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
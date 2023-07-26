using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IScriptService : IEntityService<Script>
{
}

public class ScriptService : EntityService<Script>, IScriptService
{
    public ScriptService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
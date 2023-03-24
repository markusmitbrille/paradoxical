using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface ITriggerService : IEntityService<Trigger>
{
}

public class TriggerService : EntityService<Trigger>, ITriggerService
{
    public TriggerService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

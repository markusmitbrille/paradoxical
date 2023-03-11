using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface ITriggerService : IElementService<Trigger>
{
}

public class TriggerService : ElementService<Trigger>, ITriggerService
{
    public TriggerService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

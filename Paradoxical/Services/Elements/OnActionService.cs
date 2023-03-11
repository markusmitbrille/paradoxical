using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IOnActionService : IElementService<OnAction>
{
}

public class OnActionService : ElementService<OnAction>, IOnActionService
{
    public OnActionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

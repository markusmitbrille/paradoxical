using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IDecisionService : IElementService<Decision>
{
}

public class DecisionService : ElementService<Decision>, IDecisionService
{
    public DecisionService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

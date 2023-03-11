using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IEffectService : IElementService<Effect>
{
}

public class EffectService : ElementService<Effect>, IEffectService
{
    public EffectService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
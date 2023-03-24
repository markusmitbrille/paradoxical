using Paradoxical.Core;
using Paradoxical.Model.Elements;

namespace Paradoxical.Services.Elements;

public interface IEffectService : IEntityService<Effect>
{
}

public class EffectService : EntityService<Effect>, IEffectService
{
    public EffectService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}
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

    public override void Delete(Effect model)
    {
        base.Delete(model);

        string deleteEventImmediateEffects = ParadoxQuery.CollectionDelete(
            mn: "event_immediate_effects",
            fk: "effect_id");

        string deleteEventAfterEffects = ParadoxQuery.CollectionDelete(
            mn: "event_after_effects",
            fk: "effect_id");

        Data.Connection.Execute(deleteEventImmediateEffects, model.Id);
        Data.Connection.Execute(deleteEventAfterEffects, model.Id);
    }
}
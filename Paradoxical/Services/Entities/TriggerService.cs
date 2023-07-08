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

    public override void Delete(Trigger model)
    {
        base.Delete(model);

        string deleteEventTriggers = ParadoxQuery.CollectionDelete(
            mn: "event_triggers",
            fk: "trigger_id");

        Data.Connection.Execute(deleteEventTriggers, model.Id);
    }
}

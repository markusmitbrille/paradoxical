using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOptionTriggerService : IRelationshipService<Option, Trigger, OptionTrigger>
{
}

public class OptionTriggerService : RelationshipService<Option, Trigger, OptionTrigger>, IOptionTriggerService
{
    protected override string OwnerTable => "options";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "option_id";

    protected override string RelationTable => "triggers";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "trigger_id";

    protected override string RelationshipTable => "option_triggers";

    public OptionTriggerService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

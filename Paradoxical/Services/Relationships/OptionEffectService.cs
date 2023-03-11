using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;

namespace Paradoxical.Services.Relationships;

public interface IOptionEffectService : IRelationshipService<Option, Effect, OptionEffect>
{
}

public class OptionEffectService : RelationshipService<Option, Effect, OptionEffect>, IOptionEffectService
{
    protected override string OwnerTable => "options";
    protected override string OwnerPrimaryKey => "id";
    protected override string OwnerForeignKey => "option_id";

    protected override string RelationTable => "effects";
    protected override string RelationPrimaryKey => "id";
    protected override string RelationForeignKey => "effect_id";

    protected override string RelationshipTable => "option_effects";

    public OptionEffectService(IDataService data, IMediatorService mediator) : base(data, mediator)
    {
    }
}

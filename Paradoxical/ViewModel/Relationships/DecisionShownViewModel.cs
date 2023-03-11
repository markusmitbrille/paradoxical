using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class DecisionShownViewModel : RelationshipViewModel<Decision, DecisionViewModel, Trigger, TriggerViewModel, DecisionShown, TriggerDetailsViewModel>
{
    public DecisionShownViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Trigger> relationService, IRelationshipService<Decision, Trigger, DecisionShown> relationshipService, Func<Trigger> relationFactory, Func<Trigger, TriggerViewModel> relationViewModelFactory, Func<Trigger, DecisionShown> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

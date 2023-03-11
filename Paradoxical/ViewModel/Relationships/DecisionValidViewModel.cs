using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class DecisionValidViewModel : RelationshipViewModel<Decision, DecisionViewModel, Trigger, TriggerViewModel, DecisionValid, TriggerDetailsViewModel>
{
    public DecisionValidViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Trigger> relationService, IRelationshipService<Decision, Trigger, DecisionValid> relationshipService, Func<Trigger> relationFactory, Func<Trigger, TriggerViewModel> relationViewModelFactory, Func<Trigger, DecisionValid> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

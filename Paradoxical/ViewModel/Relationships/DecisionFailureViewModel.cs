using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class DecisionFailureViewModel : RelationshipViewModel<Decision, DecisionViewModel, Trigger, TriggerViewModel, DecisionFailure, TriggerDetailsViewModel>
{
    public DecisionFailureViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Trigger> relationService, IRelationshipService<Decision, Trigger, DecisionFailure> relationshipService, Func<Trigger> relationFactory, Func<Trigger, TriggerViewModel> relationViewModelFactory, Func<Trigger, DecisionFailure> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

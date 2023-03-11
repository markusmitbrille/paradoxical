using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class OnActionTriggerViewModel : RelationshipViewModel<OnAction, OnActionViewModel, Trigger, TriggerViewModel, OnActionTrigger, TriggerDetailsViewModel>
{
    public OnActionTriggerViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Trigger> relationService, IRelationshipService<OnAction, Trigger, OnActionTrigger> relationshipService, Func<Trigger> relationFactory, Func<Trigger, TriggerViewModel> relationViewModelFactory, Func<Trigger, OnActionTrigger> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

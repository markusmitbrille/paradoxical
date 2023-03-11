using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class EventTriggerViewModel : RelationshipViewModel<Event, EventViewModel, Trigger, TriggerViewModel, EventTrigger, TriggerDetailsViewModel>
{
    public EventTriggerViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Trigger> relationService, IRelationshipService<Event, Trigger, EventTrigger> relationshipService, Func<Trigger> relationFactory, Func<Trigger, TriggerViewModel> relationViewModelFactory, Func<Trigger, EventTrigger> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

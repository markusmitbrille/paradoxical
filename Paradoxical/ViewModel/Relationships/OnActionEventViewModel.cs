using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class OnActionEventViewModel : RelationshipViewModel<OnAction, OnActionViewModel, Event, EventViewModel, OnActionEvent, EventDetailsViewModel>
{
    public OnActionEventViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Event> relationService, IRelationshipService<OnAction, Event, OnActionEvent> relationshipService, Func<Event> relationFactory, Func<Event, EventViewModel> relationViewModelFactory, Func<Event, OnActionEvent> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

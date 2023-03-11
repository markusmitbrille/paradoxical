using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class EventAfterViewModel : RelationshipViewModel<Event, EventViewModel, Effect, EffectViewModel, EventAfter, EffectDetailsViewModel>
{
    public EventAfterViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Effect> relationService, IRelationshipService<Event, Effect, EventAfter> relationshipService, Func<Effect> relationFactory, Func<Effect, EffectViewModel> relationViewModelFactory, Func<Effect, EventAfter> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

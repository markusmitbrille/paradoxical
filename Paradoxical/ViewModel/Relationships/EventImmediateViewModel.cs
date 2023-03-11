using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class EventImmediateViewModel : RelationshipViewModel<Event, EventViewModel, Effect, EffectViewModel, EventImmediate, EffectDetailsViewModel>
{
    public EventImmediateViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Effect> relationService, IRelationshipService<Event, Effect, EventImmediate> relationshipService, Func<Effect> relationFactory, Func<Effect, EffectViewModel> relationViewModelFactory, Func<Effect, EventImmediate> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

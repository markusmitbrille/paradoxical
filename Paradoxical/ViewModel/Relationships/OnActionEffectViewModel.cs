using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class OnActionEffectViewModel : RelationshipViewModel<OnAction, OnActionViewModel, Effect, EffectViewModel, OnActionEffect, EffectDetailsViewModel>
{
    public OnActionEffectViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Effect> relationService, IRelationshipService<OnAction, Effect, OnActionEffect> relationshipService, Func<Effect> relationFactory, Func<Effect, EffectViewModel> relationViewModelFactory, Func<Effect, OnActionEffect> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

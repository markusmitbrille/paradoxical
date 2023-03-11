using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class DecisionEffectViewModel : RelationshipViewModel<Decision, DecisionViewModel, Effect, EffectViewModel, DecisionEffect, EffectDetailsViewModel>
{
    public DecisionEffectViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Effect> relationService, IRelationshipService<Decision, Effect, DecisionEffect> relationshipService, Func<Effect> relationFactory, Func<Effect, EffectViewModel> relationViewModelFactory, Func<Effect, DecisionEffect> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

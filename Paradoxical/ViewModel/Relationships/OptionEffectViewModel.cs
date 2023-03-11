using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class OptionEffectViewModel : RelationshipViewModel<Option, OptionViewModel, Effect, EffectViewModel, OptionEffect, EffectDetailsViewModel>
{
    public OptionEffectViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Effect> relationService, IRelationshipService<Option, Effect, OptionEffect> relationshipService, Func<Effect> relationFactory, Func<Effect, EffectViewModel> relationViewModelFactory, Func<Effect, OptionEffect> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

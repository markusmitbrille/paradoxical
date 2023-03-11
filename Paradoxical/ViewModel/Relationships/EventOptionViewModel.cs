using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel.Relationships;

public class EventOptionViewModel : RelationshipViewModel<Event, EventViewModel, Option, OptionViewModel, EventOption, OptionDetailsViewModel>
{
    public EventOptionViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<Option> relationService, IRelationshipService<Event, Option, EventOption> relationshipService, Func<Option> relationFactory, Func<Option, OptionViewModel> relationViewModelFactory, Func<Option, EventOption> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

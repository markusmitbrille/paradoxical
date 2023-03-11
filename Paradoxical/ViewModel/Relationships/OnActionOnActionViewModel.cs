using Paradoxical.Core;
using Paradoxical.Model.Elements;
using Paradoxical.Model.Relationships;
using Paradoxical.Services;
using System;

namespace Paradoxical.ViewModel;

public class OnActionOnActionViewModel : RelationshipViewModel<OnAction, OnActionViewModel, OnAction, OnActionViewModel, OnActionOnAction, OnActionDetailsViewModel>
{
    public OnActionOnActionViewModel(NavigationViewModel navigation, FindDialogViewModel finder, IMediatorService mediator, IElementService<OnAction> relationService, IRelationshipService<OnAction, OnAction, OnActionOnAction> relationshipService, Func<OnAction> relationFactory, Func<OnAction, OnActionViewModel> relationViewModelFactory, Func<OnAction, OnActionOnAction> relationshipFactory) : base(navigation, finder, mediator, relationService, relationshipService, relationFactory, relationViewModelFactory, relationshipFactory)
    {
    }
}

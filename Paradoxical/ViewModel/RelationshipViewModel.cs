using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class RelationshipViewModel<TOwner, TOwnerViewModel, TRelation, TRelationViewModel, TRelationship, TRelationDetailsViewModel> : ViewModelBase,
    IMessageHandler<RelationAddedMessage>,
    IMessageHandler<RelationRemovedMessage>
    where TOwner : IElement
    where TOwnerViewModel : IElementViewModel
    where TRelation : IElement
    where TRelationViewModel : IElementViewModel
    where TRelationship : IRelationship
    where TRelationDetailsViewModel : PageViewModelBase
{
    public NavigationViewModel Navigation { get; }
    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public IElementService<TRelation> RelationService { get; }
    public IRelationshipService<TOwner, TRelation, TRelationship> RelationshipService { get; }

    private Func<TRelation> RelationFactory { get; }
    private Func<TRelation, TRelationViewModel> RelationViewModelFactory { get; }
    private Func<TRelation, TRelationship> RelationshipFactory { get; }

    public ObservableCollection<TRelationViewModel> Relations { get; }

    public RelationshipViewModel(
        TOwner owner,
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        IElementService<TRelation> relationService,
        IRelationshipService<TOwner, TRelation, TRelationship> relationshipService,
        Func<TRelation> relationFactory,
        Func<TRelation, TRelationViewModel> relationViewModelFactory,
        Func<TRelation, TRelationship> relationshipFactory)
    {
        Navigation = navigation;
        Finder = finder;
        Mediator = mediator;

        RelationService = relationService;
        RelationshipService = relationshipService;

        RelationFactory = relationFactory;
        RelationViewModelFactory = relationViewModelFactory;
        RelationshipFactory = relationshipFactory;

        var relations = RelationshipService.GetRelations(owner)
            .Select(RelationViewModelFactory);

        Relations = new(relations);

        Mediator.Register<RelationAddedMessage>(this);
        Mediator.Register<RelationRemovedMessage>(this);
    }

    public void Handle(RelationAddedMessage message)
    {
        if (message.Relation is not TRelationship relationship)
        { return; }

        TRelation relation = RelationService.Get(relationship.RelationID);
        TRelationViewModel relationViewModel = RelationViewModelFactory(relation);

        Relations.Add(relationViewModel);
    }

    public void Handle(RelationRemovedMessage message)
    {
        if (message.Relation is not TRelationship relationship)
        { return; }

        TRelation relation = RelationService.Get(relationship.RelationID);
        TRelationViewModel relationViewModel = RelationViewModelFactory(relation);

        Relations.Remove(relationViewModel);
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        TRelation relation = RelationFactory();
        TRelationship relationship = RelationshipFactory(relation);

        RelationService.Insert(relation);
        RelationshipService.Add(relationship);
    }

    private AsyncRelayCommand? addCommand;
    public AsyncRelayCommand AddCommand => addCommand ??= new(Add);

    private async Task Add()
    {
        Finder.Items = RelationService.Get()
            .Select(RelationViewModelFactory)
            .Except(Relations)
            .Cast<IElementViewModel>();

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        if (Finder.Selected is not TRelationViewModel relationViewModel)
        { return; }

        if (Finder.Selected.Model is not TRelation relation)
        { return; }

        TRelationship relationship = RelationshipFactory(relation);
        RelationshipService.Add(relationship);
    }

    private RelayCommand<TRelationViewModel>? removeCommand;
    public RelayCommand<TRelationViewModel> RemoveCommand => removeCommand ??= new(Remove, CanRemove);

    private void Remove(TRelationViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        if (viewmodel.Model is not TRelation relation)
        { return; }

        TRelationship relationship = RelationshipFactory(relation);
        RelationshipService.Add(relationship);

        RelationshipService.Remove(relationship);
    }
    private bool CanRemove(TRelationViewModel? viewmodel)
    {
        return viewmodel != null && viewmodel.Model is TRelation;
    }

    private RelayCommand<TRelationViewModel>? findCommand;
    public RelayCommand<TRelationViewModel> FindCommand => findCommand ??= new(Find, CanFind);

    private void Find(TRelationViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        if (viewmodel.Model is not TRelation relation)
        { return; }

        Navigation.Navigate<TRelationDetailsViewModel>();
        Mediator.Send<ElementSelectedMessage>(new(relation));
    }
    private bool CanFind(TRelationViewModel? viewmodel)
    {
        return viewmodel != null && viewmodel.Model is TRelation;
    }
}

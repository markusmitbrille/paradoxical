using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModelBase,
    IElementDetailsViewModel,
    IMessageHandler<ElementSelectedMessage>,
    IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Trigger Details";

    public FindDialogViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public ITriggerService TriggerService { get; }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    IElementViewModel? IElementDetailsViewModel.Selected => Selected;

    public TriggerDetailsViewModel(
        NavigationViewModel navigation,
        FindDialogViewModel finder,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        TriggerService = triggerService;

        Mediator.Register<ElementSelectedMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatedTo()
    {
        base.OnNavigatedTo();

        Load();
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();
    }

    public void Handle(ElementSelectedMessage message)
    {
        if (message.Element is not Trigger model)
        { return; }

        if (Navigation.CurrentPage != this)
        { return; }

        Selected = new(model);
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        if (Selected == null)
        { return; }

        Selected = new(TriggerService.Get(Selected.Model));
    }

    private void Save()
    {
        if (Selected == null)
        { return; }

        TriggerService.Update(Selected.Model);
    }

    private AsyncRelayCommand? findCommand;
    public AsyncRelayCommand FindCommand => findCommand ??= new(Find);

    private async Task Find()
    {
        Save();

        Finder.Items = TriggerService.Get().Select(model => new TriggerViewModel(model));
        Finder.Selected = Selected;

        await DialogHost.Show(Finder, Finder.DialogIdentifier);

        if (Finder.DialogResult != true)
        { return; }

        if (Finder.Selected == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();
        Mediator.Send<ElementSelectedMessage>(new(Finder.Selected.Model));
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Navigation.Navigate<TriggerDetailsViewModel>();

        Trigger model = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

        TriggerService.Insert(model);
    }

    private RelayCommand<TriggerViewModel>? duplicateCommand;
    public RelayCommand<TriggerViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();

        Trigger model = new(viewmodel.Model);
        TriggerService.Insert(model);
    }
    private bool CanDuplicate(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(TriggerViewModel? viewmodel)
    {
        if (viewmodel == null)
        { return; }

        TriggerService.Delete(viewmodel.Model);
    }
    private bool CanDelete(TriggerViewModel? viewmodel)
    {
        return viewmodel != null;
    }
}

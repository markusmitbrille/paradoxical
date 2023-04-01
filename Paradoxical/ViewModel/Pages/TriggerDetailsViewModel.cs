using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;

namespace Paradoxical.ViewModel;

public class TriggerDetailsViewModel : PageViewModel
    , IMessageHandler<SelectMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Trigger Details";

    public FinderViewModel Finder { get; }
    public IMediatorService Mediator { get; }

    public ITriggerService TriggerService { get; }

    private TriggerViewModel? selected;
    public TriggerViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public TriggerDetailsViewModel(
        NavigationViewModel navigation,
        FinderViewModel finder,
        IMediatorService mediator,
        ITriggerService triggerService)
        : base(navigation)
    {
        Finder = finder;
        Mediator = mediator;

        TriggerService = triggerService;
    }

    protected override void OnNavigatedTo()
    {
        base.OnNavigatedTo();

        Load();

        Mediator.Register<SelectMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    protected override void OnNavigatingFrom()
    {
        base.OnNavigatingFrom();

        Save();

        Mediator.Unregister<SelectMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    public void Handle(SelectMessage message)
    {
        if (message.Model is not Trigger model)
        { return; }

        var selected = TriggerService.Get(model);
        Selected = new() { Model = selected };
    }

    public void Handle(ShutdownMessage message)
    {
        Save();
    }

    private void Load()
    {
        if (Selected == null)
        { return; }

        Trigger selected = TriggerService.Get(Selected.Model);
        Selected = new() { Model = selected };
    }

    private void Save()
    {
        if (Selected == null)
        { return; }

        TriggerService.Update(Selected.Model);
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        if (Selected != null)
        {
            Navigation.Navigate<TriggerDetailsViewModel>();
        }

        Trigger model = new()
        {
            Name = $"trg_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some trigger",
        };

        TriggerService.Insert(model);
        Mediator.Send<SelectMessage>(new(model));
    }

    private RelayCommand<TriggerViewModel>? duplicateCommand;
    public RelayCommand<TriggerViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        Navigation.Navigate<TriggerDetailsViewModel>();

        Trigger model = new(observable.Model);

        TriggerService.Insert(model);
        Mediator.Send<SelectMessage>(new(model));
    }
    private bool CanDuplicate(TriggerViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<TriggerViewModel>? deleteCommand;
    public RelayCommand<TriggerViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(TriggerViewModel? observable)
    {
        if (observable == null)
        { return; }

        TriggerService.Delete(observable.Model);
    }
    private bool CanDelete(TriggerViewModel? observable)
    {
        return observable != null;
    }
}

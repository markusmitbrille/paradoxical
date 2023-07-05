using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using System;

namespace Paradoxical.ViewModel;

public class EffectDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
{
    public override string PageName => "Effect Details";

    public IEffectService EffectService { get; }

    private EffectViewModel? selected;
    public EffectViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public EffectDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IEffectService effectService)
        : base(shell, mediator)
    {
        EffectService = effectService;
    }

    protected override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
    }

    protected override void OnNavigatingFrom()
    {
        Save();

        Mediator.Unregister<SaveMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    public void Load(Effect model)
    {
        var selected = EffectService.Get(model);
        Selected = new() { Model = selected };
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        EffectService.Update(Selected.Model);
    }

    private RelayCommand? createCommand;
    public RelayCommand CreateCommand => createCommand ??= new(Create);

    private void Create()
    {
        Effect model = new()
        {
            Name = $"eff_{Guid.NewGuid().ToString("N").Substring(0, 4)}",
            Code = "# some effect",
        };

        EffectService.Insert(model);

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }

    private RelayCommand<EffectViewModel>? duplicateCommand;
    public RelayCommand<EffectViewModel> DuplicateCommand => duplicateCommand ??= new(Duplicate, CanDuplicate);

    private void Duplicate(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        Effect model = new(observable.Model);

        EffectService.Insert(model);

        var page = Shell.Navigate<EffectDetailsViewModel>();
        page.Load(model);
    }
    private bool CanDuplicate(EffectViewModel? observable)
    {
        return observable != null;
    }

    private RelayCommand<EffectViewModel>? deleteCommand;
    public RelayCommand<EffectViewModel> DeleteCommand => deleteCommand ??= new(Delete, CanDelete);

    private void Delete(EffectViewModel? observable)
    {
        if (observable == null)
        { return; }

        EffectService.Delete(observable.Model);
    }
    private bool CanDelete(EffectViewModel? observable)
    {
        return observable != null;
    }
}

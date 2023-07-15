using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Services;
using Paradoxical.Services.Elements;
using Paradoxical.Services.Entities;
using System.Linq;

namespace Paradoxical.ViewModel;

public class ModDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
    , IMessageHandler<ShutdownMessage>
{
    public override string PageName => "Mod Info";

    public IModService ModService { get; }

    private int selectedTab;
    public int SelectedTab
    {
        get => selectedTab;
        set => SetProperty(ref selectedTab, value);
    }

    private ModViewModel? selected;
    public ModViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public ModDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IModService modService)
        : base(shell, mediator)
    {
        ModService = modService;
    }

    public override void OnNavigatedTo()
    {
        Reload();

        Mediator.Register<SaveMessage>(this);
        Mediator.Register<ShutdownMessage>(this);
    }

    public override void OnNavigatingFrom()
    {
        Save();

        Mediator.Unregister<SaveMessage>(this);
        Mediator.Unregister<ShutdownMessage>(this);
    }

    void IMessageHandler<SaveMessage>.Handle(SaveMessage message)
    {
        Save();
    }

    void IMessageHandler<ShutdownMessage>.Handle(ShutdownMessage message)
    {
        Save();
    }

    public void Load()
    {
        var selected = ModService.Get().SingleOrDefault();
        if (selected == null)
        {
            selected = new();
            ModService.Insert(selected);
        }

        Selected = new() { Model = selected };
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload);

    private void Reload()
    {
        Load();
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save);

    private void Save()
    {
        if (Selected == null)
        { return; }

        ModService.Update(Selected.Model);
    }
}
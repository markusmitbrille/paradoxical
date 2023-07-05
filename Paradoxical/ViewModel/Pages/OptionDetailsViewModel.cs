using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Model.Elements;
using Paradoxical.Services;
using Paradoxical.Services.Elements;

namespace Paradoxical.ViewModel;

public class OptionDetailsViewModel : PageViewModel
    , IMessageHandler<SaveMessage>
{
    public override string PageName => "Option Details";

    public IOptionService OptionService { get; }

    private OptionViewModel? selected;
    public OptionViewModel? Selected
    {
        get => selected;
        set => SetProperty(ref selected, value);
    }

    public OptionDetailsViewModel(
        IShell shell,
        IMediatorService mediator,
        IOptionService service)
        : base(shell, mediator)
    {
        OptionService = service;
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

    public void Load(Option model)
    {
        var selected = OptionService.Get(model);
        Selected = new() { Model = selected };
    }

    private RelayCommand? reloadCommand;
    public RelayCommand ReloadCommand => reloadCommand ??= new(Reload, CanReload);

    private void Reload()
    {
        if (Selected == null)
        { return; }

        Load(Selected.Model);
    }
    private bool CanReload()
    {
        return Selected != null;
    }

    private RelayCommand? saveCommand;
    public RelayCommand SaveCommand => saveCommand ??= new(Save, CanSave);

    private void Save()
    {
        if (Selected == null)
        { return; }

        OptionService.Update(Selected.Model);
    }
    private bool CanSave()
    {
        return Selected != null;
    }
}
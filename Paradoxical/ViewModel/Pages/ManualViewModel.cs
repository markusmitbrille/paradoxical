using CommunityToolkit.Mvvm.Input;
using Paradoxical.Core;
using Paradoxical.Messages;
using Paradoxical.Services;
using System.Windows;

namespace Paradoxical.ViewModel;

public class ManualViewModel : PageViewModel
{
    public override string PageName => "Manual";

    public ManualViewModel(IShell shell, IMediatorService mediator)
        : base(shell, mediator)
    {
    }

    private RelayCommand<FrameworkContentElement>? goToCommand;
    public RelayCommand<FrameworkContentElement> GoToCommand => goToCommand ??= new(GoTo);

    private void GoTo(FrameworkContentElement? element)
    {
        if (element == null)
        { return; }

        element.BringIntoView();
    }
}

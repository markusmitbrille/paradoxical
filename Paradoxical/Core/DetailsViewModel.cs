using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Services;
using Paradoxical.ViewModel;

namespace Paradoxical.Core;

public abstract class DetailsViewModel : ObservableObject
{
    public IShell Shell { get; }
    public IMediatorService Mediator { get; }

    public DetailsViewModel(
        IShell shell,
        IMediatorService mediator)
    {
        Shell = shell;
        Mediator = mediator;
    }
}

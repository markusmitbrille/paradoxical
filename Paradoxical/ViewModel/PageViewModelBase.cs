using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel;

public abstract class PageViewModelBase : ObservableObject
{
    public abstract string PageName { get; }
}

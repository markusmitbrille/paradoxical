using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel
{
    public abstract partial class PageViewModelBase : ObservableObject
    {
        public abstract string PageName { get; }
    }
}
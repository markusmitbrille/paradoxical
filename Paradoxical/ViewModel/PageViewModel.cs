using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel
{
    public abstract partial class PageViewModel : ObservableObject
    {
        public abstract string PageName { get; }
    }
}
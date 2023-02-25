using CommunityToolkit.Mvvm.ComponentModel;

namespace Paradoxical.ViewModel;

public partial class MainViewModel : ObservableObject
{
    public ApplicationViewModel App { get; }

    public MainViewModel(ApplicationViewModel app)
    {
        App = app;
    }
}
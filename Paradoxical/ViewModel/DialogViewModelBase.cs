using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace Paradoxical.ViewModel
{
    public abstract partial class DialogViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool? dialogResult;

        [ObservableProperty]
        private string? dialogIdentifier;

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private void Submit()
        {
            DialogResult = true;

            Close();
        }
        protected virtual bool CanSubmit()
        {
            return true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;

            Close();
        }

        [RelayCommand]
        private void Close()
        {
            if (DialogHost.IsDialogOpen(DialogIdentifier))
            { DialogHost.Close(DialogIdentifier); }
        }
    }
}
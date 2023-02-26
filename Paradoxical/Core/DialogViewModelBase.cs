using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;

namespace Paradoxical.Core;

public abstract class DialogViewModelBase : ViewModelBase
{
    private bool? dialogResult;
    public bool? DialogResult
    {
        get => dialogResult;
        set => SetProperty(ref dialogResult, value);
    }

    private string? dialogIdentifier;
    public string? DialogIdentifier
    {
        get => dialogIdentifier;
        set => SetProperty(ref dialogIdentifier, value);
    }

    private RelayCommand? submitCommand;
    public RelayCommand SubmitCommand => submitCommand ??= new(Submit, CanSubmit);

    private void Submit()
    {
        DialogResult = true;

        Close();
    }
    protected virtual bool CanSubmit()
    {
        return true;
    }

    private RelayCommand? cancelCommand;
    public RelayCommand CancelCommand => cancelCommand ??= new(Cancel);

    private void Cancel()
    {
        DialogResult = false;

        Close();
    }

    private RelayCommand? closeCommand;
    public RelayCommand CloseCommand => closeCommand ??= new(Close);

    private void Close()
    {
        if (DialogHost.IsDialogOpen(DialogIdentifier))
        { DialogHost.Close(DialogIdentifier); }
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows;

namespace Paradoxical.Core;

public abstract class DialogViewModel<T> : ObservableObject
    where T : Window, new()
{
    private T? Window { get; set; }

    public bool? Show()
    {
        Window = new T() { DataContext = this };

        Window.Deactivated += Window_Deactivated;
        Window.Closing += Window_Closing;

        return Window.ShowDialog();
    }

    private void Window_Deactivated(object? sender, EventArgs e)
    {
        if (Window == null)
        { return; }

        Window.Close();
    }

    private void Window_Closing(object? sender, CancelEventArgs e)
    {
        if (Window == null)
        { return; }

        Window.Deactivated -= Window_Deactivated;
        Window.Closing -= Window_Closing;
    }

    private RelayCommand? submitCommand;
    public RelayCommand SubmitCommand => submitCommand ??= new(Submit, CanSubmit);

    private void Submit()
    {
        if (Window == null)
        { return; }

        if (Window.IsActive == false)
        { return; }

        Window.DialogResult = true;
    }
    private bool CanSubmit()
    {
        return true;
    }

    private RelayCommand? cancelCommand;
    public RelayCommand CancelCommand => cancelCommand ??= new(Cancel);

    private void Cancel()
    {
        if (Window == null)
        { return; }

        if (Window.IsActive == false)
        { return; }

        Window.DialogResult = false;
    }

    private RelayCommand? closeCommand;
    public RelayCommand CloseCommand => closeCommand ??= new(Close);

    private void Close()
    {
        if (Window == null)
        { return; }

        if (Window.IsActive == false)
        { return; }

        Window.Close();
    }
}

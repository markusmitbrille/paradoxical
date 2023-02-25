using CommunityToolkit.Mvvm.ComponentModel;
using Paradoxical.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paradoxical.ViewModel;

public partial class TriggerViewModel : ObservableObject
{
    private readonly Trigger model;
    public Trigger Model => model;

    public int Id
    {
        get => model.Id;
    }

    public string Name
    {
        get => model.name;
        set => SetProperty(ref model.name, value);
    }

    public string Code
    {
        get => model.code;
        set => SetProperty(ref model.code, value);
    }

    public string Tooltip
    {
        get => model.tooltip;
        set => SetProperty(ref model.tooltip, value);
    }

    public TriggerViewModel(Trigger model)
    {
        this.model = model;
    }
}
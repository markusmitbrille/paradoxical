using Paradoxical.Core;
using Paradoxical.Model;
using System.Collections.Generic;

namespace Paradoxical.ViewModel;

public partial class TriggerViewModel : ViewModelBase, IModelViewModel, IElementViewModel
{
    private static readonly Dictionary<Trigger, TriggerViewModel> cache = new();
    public static TriggerViewModel Get(Trigger model)
    {
        if (cache.TryGetValue(model, out var viewModel) == false)
        {
            viewModel = new(model);
            cache.Add(model, viewModel);
        }

        return viewModel;
    }

    private readonly Trigger model;
    public Trigger Model => model;

    IModel IModelViewModel.Model => Model;
    IElement IElementViewModel.Model => Model;

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
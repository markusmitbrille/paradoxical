using Paradoxical.Model;
using Paradoxical.ViewModel;
using System;

namespace Paradoxical.Core;

public interface IElementViewModel
{
    public IElementModel Model { get; }

    public int Id { get; }
    public string Name { get; }
}

public static class ElementViewModel
{
    public static IElementViewModel Get(IElementModel model)
    {
        switch (model)
        {
            case Decision decision:
                return DecisionViewModel.Get(decision);
            default:
                throw new NotImplementedException($"Not implemented for {model.GetType().Name}!");
        }
    }
}

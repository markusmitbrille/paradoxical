using Paradoxical.Model;
using System;

namespace Paradoxical.ViewModel;

public static class ElementViewModel
{
    public static IElementViewModel Get(IElement model)
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

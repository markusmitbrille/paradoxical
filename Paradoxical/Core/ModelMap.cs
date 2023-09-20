using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Core;

public class ModelMap<M, W>
    where M : IModel, new()
    where W : IViewModel<M>, new()
{
    private Dictionary<M, W> Map { get; } = new();

    public W this[M model]
    {
        get
        {
            if (Map.ContainsKey(model) == true)
            {
                return Map[model];
            }

            var vm = new W() { Model = model };
            Map[model] = vm;

            return vm;
        }
    }

    public IEnumerable<M> Models => Map.Keys;
    public IEnumerable<W> ViewModels => Map.Values;

    public ModelMap()
    {
    }

    public ModelMap(IEnumerable<M> models)
    {
        Map = models.ToDictionary(model => model, model => new W() { Model = model });
    }

    public void Remove(M model)
    {
        Map.Remove(model);
    }
}

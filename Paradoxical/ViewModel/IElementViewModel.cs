using Paradoxical.Model;

namespace Paradoxical.ViewModel;

public interface IElementViewModel
{
    public IElement Model { get; }

    public int Id { get; }
    public string Name { get; }
}

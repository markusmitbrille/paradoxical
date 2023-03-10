namespace Paradoxical.Core;

public interface IComponent
{
    public int Id { get; }
    public int OwnerId { get; }
}

public interface IComponentViewModel
{
    public IComponent Model { get; }
}

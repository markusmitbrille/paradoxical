namespace Paradoxical.Core;

public interface IModel
{
    public int Id { get; }
}

public interface IModelViewModel
{
    public IModel Model { get; }

    public int Id { get; }
}

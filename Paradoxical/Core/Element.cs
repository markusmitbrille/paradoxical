namespace Paradoxical.Core;

public interface IElement
{
    public int Id { get; }
    public string Name { get; }
}

public interface IElementViewModel
{
    public IElement Model { get; }

    public int Id { get; }
    public string Name { get; }
}

public interface IElementTableViewModel
{
    public IElementViewModel Selected { get; }
}

public interface IElementDetailsViewModel
{
    public IElementViewModel Selected { get; }
}
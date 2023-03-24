namespace Paradoxical.Core;

public interface IElementWrapper : IModelWrapper
{
    string Kind { get; }
    string Name { get; set; }
}

public interface IElementWrapper<T> : IModelWrapper<T>
    where T : IElement
{
    string Kind { get; }
    string Name { get; set; }
}

public abstract class ElementWrapper<T> : ModelWrapper<T>
    , IElementWrapper
    , IElementWrapper<T>
    where T : IElement
{
    public abstract string Kind { get; }
    public abstract string Name { get; set; }

    public ElementWrapper(T model) : base(model)
    {
    }
}
